import sys,time,json,statistics
from pathlib import Path
sys.path.insert(0,'/home/kevin/modding/pvzreplanted/debug-mod/tools')
from debug_api import call,GAME,ApiFailure
label=sys.argv[1];family=sys.argv[2] if len(sys.argv)>2 else 'native'
root=GAME/'UserData/DebugModApi';out=Path('/home/kevin/modding/pvzreplanted/.test-runs/projectile-performance-20261106')
events=[];stages=[]
def api(cmd,args={}):
 r=call(root,cmd,args);events.append(dict(at=time.monotonic(),command=cmd,args=args,response=r));assert r['ok'],r;return r['result']
def wait(cmd,pred,seconds=45):
 end=time.monotonic()+seconds;r=None
 while time.monotonic()<end:
  try:
   r=call(root,cmd,{})
   if r.get('ok') and pred(r['result']):events.append(dict(command=cmd,response=r));return r['result']
  except ApiFailure:pass
  time.sleep(.2)
 raise AssertionError((cmd,r))
def samplefps():
 a=time.monotonic();v=api('fps');b=time.monotonic();return dict(at=(a+b)/2,**v)
def counts():
 p=api('projectiles')['items'];types={}
 for item in p:types[item['mProjectileType']]=types.get(item['mProjectileType'],0)+1
 return dict(count=len(p),types=types)
try:
 state=wait('status',lambda s:s.get('debug') and s.get('scene')=='Playing',180);board=state['board']
 settings=api('settings');api('difficulty',dict(board=board,value=3));api('pause',dict(paused=True));api('speed',dict(index=0))
 for kind in ('plant','zombie'):
  for item in api(kind+'s')['items']:
   v=item.get('native',item);api(kind+'.remove',dict(board=board,id=v['DataID']))
 for row in range(5):
  z=api('zombie.place',dict(type='Normal',col=8,row=row))['native'];assert z['mGraveX']==0
  api('debug.zombie.set',dict(board=board,id=z['DataID'],health=1000000,freezeTicks=1000000))
 placed=0;cells=[(col,row) for row in range(5) for col in range(7)]
 for size in ((35,) if label=='profile-v2' else (0,7,21,35)):
  api('pause',dict(paused=True))
  for col,row in cells[placed:size]:
   if family=='native':
    if label=='nofusion':api('plant.place',dict(type='Repeater',col=col,row=row))
    p=api('plant.place',dict(type='Gatlingpea',col=col,row=row))['plants'][-1]
    assert p['mSeedType']=='Gatlingpea' and p['mImitaterType']=='None',p
   else:raise AssertionError('Unknown shooter family')
  placed=size;api('pause',dict(paused=False));time.sleep(8)
  samples=[];population=[];before=api('zombies');start=time.monotonic();first=samplefps();samples.append(first)
  while time.monotonic()-start<20:
   if len(samples)%10==0:population.append(counts())
   time.sleep(.45);samples.append(samplefps())
  last=samples[-1];population.append(counts());after=api('zombies')
  elapsed=last['at']-first['at'];fps=(last['frame']-first['frame'])/elapsed;tps=(last['simulationTick']-first['simulationTick'])/elapsed
  stage=dict(plants=size,zombies=5,scale=1,elapsed=elapsed,framesPerSecond=fps,ticksPerSecond=tps,simulationSecondsPerSecond=tps*last['tickSeconds'],instantaneousFPSMedian=statistics.median(x['instantaneous'] for x in samples),projectiles=population,samples=samples,zombiesBefore=before,zombiesAfter=after)
  stages.append(stage);(out/(label+'-'+family+'.json')).write_text(json.dumps(dict(label=label,family=family,settings=settings,stages=stages),indent=2)+'\n')
  print(json.dumps({k:stage[k] for k in ('plants','framesPerSecond','ticksPerSecond','simulationSecondsPerSecond')}), 'projectiles', [p['count'] for p in population],flush=True)
 api('pause',dict(paused=True))
 print('BASELINE COMPLETE; board paused.',flush=True)
except BaseException as e:
 events.append(dict(failed=True,error=str(e)));raise
finally:
 (out/(label+'-'+family+'-events.json')).write_text(json.dumps(events,indent=2)+'\n')
