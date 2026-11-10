"""Reject invalid timing writes, dispose the debug board and quit regularly.

Run only after explicit session handoff. Preserve raw responses incrementally,
including when a later assertion fails. Match actual process executable names,
not source text inside a shell command line.
"""
import json
from pathlib import Path
import sys
import time
PROJECT=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(PROJECT.parent/"debug-mod/tools"))
from debug_api import GAME,call
API=GAME/"UserData/DebugModApi"
OUT=PROJECT/"references/tick-rate-migration/runtime/lifecycle-runtime.json"
events=[]
def api(command,**args):
    result=call(API,command,args)
    events.append(dict(command=command,args=args,response=result))
    OUT.parent.mkdir(parents=True,exist_ok=True)
    OUT.write_text(json.dumps(dict(events=events,complete=False),indent=2))
    return result
def ok(command,**args):
    result=api(command,**args);assert result["ok"],result;return result["result"]
def actual_processes():
    result=[]
    for proc in Path("/proc").iterdir():
        if not proc.name.isdigit():continue
        try:
            comm=(proc/"comm").read_text().strip()
            argv=(proc/"cmdline").read_bytes().split(bytes([0]))
        except (OSError,ProcessLookupError):continue
        executable=Path(argv[0].decode(errors="replace")).name if argv and argv[0] else ""
        if comm=="Replanted.exe" or executable=="Replanted.exe":
            result.append(dict(pid=int(proc.name),comm=comm,executable=executable))
    return result
def main():
    state=ok("status");board=state["board"]
    assert state["debug"] and not ok("plants")["items"] and not ok("zombies")["items"] and not ok("projectiles")["items"]
    settings=ok("settings");ok("pause",paused=True)
    original=ok("debug.timing");assert original["hz"]==120 and not original["overridden"]
    invalid=api("debug.timing",board=board,hz=110);assert not invalid["ok"] and invalid["error"]["code"]=="INVALID_ARGUMENT"
    assert ok("debug.timing")==original
    stale=api("debug.timing",board="stale-"+board,hz=100);assert not stale["ok"] and stale["error"]["code"]=="STALE_BOARD"
    assert ok("debug.timing")==original
    override=ok("debug.timing",board=board,hz=100);assert override["hz"]==100 and override["overridden"]
    ok("level.leave")
    end=time.monotonic()+20
    while time.monotonic()<end:
        after=ok("status")
        if not after.get("board"):break
        time.sleep(.1)
    assert not after.get("board") and not after.get("debug"),after
    fps=ok("fps");assert abs(fps["tickSeconds"]-1/120)<1e-8,fps
    rejected=api("debug.timing",board=board,hz=100);assert not rejected["ok"] and rejected["error"]["code"] in ("BOARD_NOT_READY","DEBUG_REQUIRED")
    assert ok("settings")==settings
    before_processes=actual_processes();assert before_processes
    shutdown=ok("game.shutdown")
    end=time.monotonic()+30
    while time.monotonic()<end:
        remaining=actual_processes()
        if not remaining:break
        time.sleep(.2)
    assert not remaining,remaining
    OUT.write_text(json.dumps(dict(events=events,complete=True,original=original,afterDispose=after,fps=fps,settingsIdentical=True,beforeProcesses=before_processes,remainingProcesses=remaining,shutdown=shutdown),indent=2))
    print(json.dumps(dict(complete=True,events=len(events),remainingProcesses=remaining,evidence=str(OUT))))
if __name__=="__main__":main()
