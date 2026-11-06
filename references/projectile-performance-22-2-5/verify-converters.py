import sys,time,json,shutil
from pathlib import Path
sys.path.insert(0,'/home/kevin/modding/pvzreplanted/debug-mod/tools')
from debug_api import call,GAME
root=GAME/'UserData/DebugModApi'
out=Path('/home/kevin/modding/pvzreplanted/.test-runs/projectile-performance-20261106')
events=[];checks=[]
def api(cmd,**args):
 r=call(root,cmd,args)
 if cmd not in ('projectiles','plants','zombies'):events.append(dict(command=cmd,args=args,response=r))
 assert r['ok'],r
 return r['result']
def wait(cmd,pred,seconds=25,**args):
 end=time.monotonic()+seconds
 while time.monotonic()<end:
  r=api(cmd,**args);v=pred(r)
  if v:return v
  time.sleep(.03)
 raise AssertionError((cmd,r))
state=api('status');assert state['debug'] and state['scene']=='Playing'
board=state['board'];api('difficulty',board=board,value=3)
catalog=api('catalog.plants')['registered']
def place(seed,col,row=0,marker=None):
 args=dict(col=col,row=row)
 if marker:args['registered']=next(x['index'] for x in catalog if x['type']==seed and x['marker']==marker)
 else:args['type']=seed
 return api('plant.place',**args)['plants'][-1]
def clear():
 api('pause',paused=True)
 for z in api('zombies')['items']:api('zombie.remove',board=board,id=z['native']['DataID'])
 for p in api('plants')['items']:api('plant.remove',board=board,id=p['DataID'])
 api('speed',index=5);api('pause',paused=False)
 wait('projectiles',lambda r:not r['items'])
 api('pause',paused=True);api('speed',index=0)
try:
 for name,seed,marker,wood,woodmark,tag,model in [
  ('Nutwood','Peashooter',None,'Torchwood','Wallnut',None,None),
  ('HotPotato','Puffshroom','Peashooter','Potatomine','Torchwood',-624010,'SmallFireProjectile'),
  ('Scorchwood','Peashooter',None,'Torchwood','Jalapeno',-622001,None),
  ('Stenchwood','Peashooter',None,'Torchwood','Garlic',-622002,None),
  ('Nutwood-recreated','Peashooter',None,'Torchwood','Wallnut',None,None)]:
  clear();place(seed,0,marker=marker);p=place(wood,2,marker=woodmark)
  if name=='Nutwood-recreated':
   api('plant.move',board=board,id=p['DataID'],col=3,row=1)
   api('plant.move',board=board,id=p['DataID'],col=3,row=0)
  if seed=='Puffshroom':place('InstantCoffee',0)
  z=api('zombie.place',type='Normal',col=8,row=0)['native'];zid=z['DataID']
  api('debug.zombie.set',board=board,id=zid,health=100000,freezeTicks=100000)
  api('pause',paused=False)
  shot=wait('projectiles',lambda r:next((s for s in r['items'] if s['mPosX']>310 and ((tag is None and s['mProjectileType'] in ('Fireball','PeashooterFireball')) or (tag is not None and s['mCobTargetX']==tag))),None),visuals=True)
  if model:assert any(model in (v['path'] or '') and v['enabled'] and v['active'] for v in shot['visuals']),shot
  assert any(v['enabled'] and v['active'] for v in shot['visuals']),shot
  hit=wait('zombies',lambda r:next((v['native'] for v in r['items'] if v['native']['DataID']==zid and v['native']['mBodyHealth']<100000),None))
  expected={'Nutwood':40,'HotPotato':40,'Scorchwood':50,'Stenchwood':30,'Nutwood-recreated':40}[name]
  assert 100000-hit['mBodyHealth']==expected,(name,hit)
  api('pause',paused=True);checks.append(dict(name=name,plant=p,projectile=shot,zombieAfter=hit));print('PASS',name,'damage',100000-hit['mBodyHealth'],flush=True)
 clear();api('pause',paused=True)
finally:
 (out/(sys.argv[1] if len(sys.argv)>1 else 'converter-review.json')).write_text(json.dumps(dict(checks=checks,events=events),indent=2)+'\n')
