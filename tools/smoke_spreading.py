"""Explicit live smoke test. Requires an already playing temporary debug level.

Clears only that temporary board; never opens ordinary levels or writes profiles.
Start a Night debug level with Debug Mod API, then run this file explicitly.
"""
import sys,json
from pathlib import Path
PROJECT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(PROJECT.parent / "debug-mod/tools"))
from debug_api import call,GAME
root=GAME/"UserData/DebugModApi"; out=[]
status = call(root, "status", {})
assert status["ok"] and status["result"]["debug"] and status["result"]["scene"] == "Playing", "An active temporary debug level is required."
def api(c,a={}):
 r=call(root,c,a); out.append({"command":c,"args":a,"response":r}); assert r["ok"],r; return r["result"]
def clean():
 b=api("status")["board"]
 for p in api("plants")["items"]: api("plant.remove",{"board":b,"id":p["DataID"]})
try:
 for seed in ("Fumeshroom","Scaredyshroom"):
  for count in (1,2,3):
   clean()
   for _ in range(count): api("plant.place",{"type":"Puffshroom","col":2,"row":2})
   api("plant.place",{"type":seed,"col":2,"row":2})
   p=api("plants")["items"]; counts={}
   for v in p: counts[(v["mPlantCol"],v["mRow"])]=counts.get((v["mPlantCol"],v["mRow"]),0)+1
   expected={(2,2):1}
   if seed=="Fumeshroom": expected.update({(2+d,2):count-d+1 for d in range(1,count+1)})
   else:
    expected[(3,2)]=max(1,count-1)
    if count>1: expected.update({(2,1):count-1,(2,3):count-1})
   assert counts==expected,(seed,count,counts,expected)
   assert sum(v["mSeedType"]==seed and v["mImitaterType"]=="None" for v in p)==1
   print(seed,count,"OK",counts,flush=True)
 # Blocked cells do not move their counts into later cells.
 clean()
 api("plant.place", {"type":"Wallnut", "col":3, "row":2})
 for _ in range(3): api("plant.place", {"type":"Puffshroom", "col":2, "row":2})
 api("plant.place", {"type":"Fumeshroom", "col":2, "row":2})
 p=api("plants")["items"]
 assert sum(v["mSeedType"]=="Puffshroom" for v in p)==3
 assert sum(v["mSeedType"]=="Wallnut" for v in p)==1
 print("Blocked spread cell OK", flush=True)
 # Existing small occupants retain their identities; only free slots fill.
 clean()
 for _ in range(2): api("plant.place", {"type":"Puffshroom", "col":3, "row":2})
 for _ in range(3): api("plant.place", {"type":"Puffshroom", "col":2, "row":2})
 api("plant.place", {"type":"Fumeshroom", "col":2, "row":2})
 p=api("plants")["items"]
 assert sum(v["mPlantCol"]==3 and v["mRow"]==2 for v in p)==3
 print("Partial destination capacity OK", flush=True)
 # Both board edges, with exact source quotas and no out-of-board objects.
 for seed, col, row, expected in (("Fumeshroom", 8, 1, 1), ("Scaredyshroom", 0, 0, 5)):
  clean()
  for _ in range(3): api("plant.place", {"type":"Puffshroom", "col":col, "row":row})
  api("plant.place", {"type":seed, "col":col, "row":row})
  p=api("plants")["items"]
  assert len(p)==expected
  assert all(0<=v["mPlantCol"]<=8 and 0<=v["mRow"]<5 for v in p)
 print("Board edges OK", flush=True)
 # Glove entry point consumes its remote source and the whole target cluster.
 for seed in ("Fumeshroom", "Scaredyshroom"):
  clean()
  for _ in range(3): api("plant.place", {"type":"Puffshroom", "col":2, "row":2})
  source=api("plant.place", {"type":seed, "col":0, "row":4})["plants"][0]
  api("plant.move", {"board":api("status")["board"], "id":source["DataID"], "col":2, "row":2})
  p=api("plants")["items"]
  assert len(p)==7 and sum(v["mSeedType"]==seed for v in p)==1
  assert not any(v["DataID"]==source["DataID"] for v in p)
 print("Both glove fusions OK", flush=True)
 # Sun-shrooms and variants cannot be consumed by the plain-Puff recipe.
 clean()
 api("plant.place", {"type":"Sunshroom", "col":2, "row":2})
 api("plant.place", {"type":"Puffshroom", "col":2, "row":2})
 rejected=call(root,"plant.place",{"type":"Fumeshroom", "col":2, "row":2})
 out.append({"mixedClusterRejected":rejected})
 assert not rejected["ok"] and rejected["error"]["code"]=="INVALID_PLACEMENT"
 assert len(api("plants")["items"])==2
 print("Mixed cluster protection OK", flush=True)
finally:
 (PROJECT / "references/spreading-21-0-0/runtime.json").write_text(json.dumps(out,indent=2))
