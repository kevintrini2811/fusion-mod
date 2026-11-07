"""Focused real-game regression for the allied Hypno Chomper meal issue."""
import json, shutil, sys, time
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(ROOT.parent/'debug-mod/tools'))
from debug_api import GAME,call
API=GAME/'UserData/DebugModApi';OUT=Path(__file__).resolve().parent;events=[];original_difficulty=None;b=None
def api(command,**args):
 r=call(API,command,args);events.append(dict(command=command,args=args,response=r));assert r['ok'],r;return r['result']
def wait(command,predicate):
 end=time.monotonic()+20
 while time.monotonic()<end:
  value=api(command)
  if found:=predicate(value):return found
  time.sleep(.03)
 raise AssertionError((command,value))
try:
 s=api('status');assert s['debug'] and s['scene']=='Playing',s;b=s['board']
 original_difficulty=api('difficulty')['value'];api('difficulty',board=b,value=3)
 api('pause',paused=True)
 for z in api('zombies')['items']:api('zombie.remove',board=b,id=z['native']['DataID'])
 for p in api('plants')['items']:api('plant.remove',board=b,id=p['DataID'])
 api('plant.place',type='Hypnoshroom',col=5,row=1)
 z=api('zombie.place',type='Normal',col=5,row=1)['native']
 api('pause',paused=False)
 friend=wait('zombies',lambda v:next((q['native'] for q in v['items'] if q['native']['DataID']==z['DataID'] and q['native']['mMindControlled']),None))
 api('pause',paused=True);api('zombie.move',board=b,id=friend['DataID'],col=2,row=1)
 card=next(q for q in api('catalog.plants')['registered'] if q['type']=='Chomper' and q['marker']=='Hypnoshroom')
 p=api('plant.place',registered=card['index'],col=2,row=1)['plants'][0]
 before={q['DataID'] for q in api('coins')['items']};api('pause',paused=False)
 digest=wait('plants',lambda v:next((q for q in v['items'] if q['DataID']==p['DataID'] and q['mState']=='ChomperDigesting'),None))
 api('pause',paused=True)
 assert 900<=digest['mStateCountdown']<=1000,digest
 packets=[q for q in api('coins')['items'] if q['DataID'] not in before and q.get('mUsableSeedType')=='Hypnoshroom']
 assert len(packets)==1,packets
 shot=api('screenshot');shutil.copy2(shot['path'],OUT/'allied-meal-fixed.png')
 print('PASS native allied devour: short digestion',digest['mStateCountdown'],'and exactly one Hypno-shroom packet',flush=True)
finally:
 if original_difficulty is not None:api('difficulty',board=b,value=original_difficulty)
 (OUT/'allied-meal-fixed.json').write_text(json.dumps(events,indent=2))
