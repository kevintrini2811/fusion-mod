import sys,time,json,shutil
from pathlib import Path
sys.path.insert(0,'/home/kevin/modding/pvzreplanted/debug-mod/tools')
from debug_api import call,GAME,ApiFailure
root=GAME/'UserData/DebugModApi';label=sys.argv[1]
out=Path('/home/kevin/modding/pvzreplanted/.test-runs/eyes-preview-review-20261106');events=[];checks=[]
def api(cmd,**args):
 r=call(root,cmd,args);events.append(dict(command=cmd,args=args,response=r));assert r['ok'],r;return r['result']
def waitready():
 end=time.monotonic()+180
 while time.monotonic()<end:
  try:
   r=call(root,'status',{})
   if r.get('ok') and r['result']['debug'] and r['result']['scene']=='Playing':return r['result']
  except ApiFailure:pass
  time.sleep(.3)
 raise AssertionError('No playing debug board')
def capture(name):
 p=api('screenshot')['path'];shutil.copy2(p,out/(label+'-'+name+'.png'))
try:
 status=waitready();board=status['board'];api('difficulty',board=board,value=3);api('speed',index=0)
 catalog=api('catalog.zombies')['debug'];eyes=[v for v in catalog if 'Augenzombie' in v['name'] or 'Eyes Zombie' in v['name']]
 assert len(eyes)==3,eyes
 selected=[0]+[v['index'] for v in eyes];api('debug.roster',indices=selected)
 for row in (0,2):
  for entry in [catalog[0]]+eyes:
   api('debug.preview',cancel=True)
   for z in api('zombies')['items']:api('zombie.remove',board=board,id=z['native']['DataID'])
   preview=api('debug.preview',index=entry['index'],col=3,row=row)
   time.sleep(.3);preview=api('debug.preview')
   render=preview['rendering'];assert render and render['enabled'] and render['active'],preview
   name=str(entry['index'])+'-row'+str(row)
   capture(name+'-preview')
   api('debug.preview',place=True);time.sleep(.1)
   zombies=api('zombies',visuals=True)['items'];assert len(zombies)==1,zombies
   capture(name+'-placed')
   check=dict(entry=entry,row=row,preview=preview,placed=zombies[0]);checks.append(check)
   (out/(label+'-runtime.json')).write_text(json.dumps(dict(checks=checks,events=events),indent=2)+'\n')
   print('PASS preview and placement',entry['name'],'row',row,'anchor',render['anchor'],'pivot',render['pivot'],'foot',render['foot'],flush=True)
 api('debug.preview',cancel=True)
 print('COMPLETE',label,flush=True)
finally:(out/(label+'-runtime.json')).write_text(json.dumps(dict(checks=checks,events=events),indent=2)+'\n')
