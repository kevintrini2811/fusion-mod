"""Check debug chooser paging using the actual virtual controller and native UI."""
import json
from pathlib import Path
import shutil
import sys
import time

sys.path.insert(0, str(Path(__file__).resolve().parents[2] / 'debug-mod/tools'))
from debug_api import call, GAME, ApiFailure

OUT = Path(__file__).resolve().parents[1] / 'references/umbrella-ice-21-5-0'
ROOT = GAME / 'UserData/DebugModApi'
events = []
checks = []

def api(command, **args):
    response = call(ROOT, command, args)
    events.append(dict(command=command, args=args, response=response))
    assert response['ok'], response
    return response['result']

def wait(command, predicate, seconds=90):
    end = time.monotonic() + seconds
    while time.monotonic() < end:
        response = call(ROOT, command, {})
        events.append(dict(command=command, args={}, response=response))
        if not response['ok']:
            assert response['error']['code'] in ('BOARD_NOT_READY', 'CHOOSER_NOT_READY'), response
            time.sleep(.2)
            continue
        value = response['result']
        if predicate(value):
            return value
        time.sleep(.2)
    raise AssertionError((command, value))

def check(condition, name):
    assert condition, name
    checks.append(name)
    print('PASS', name, flush=True)

def move(button):
    api('controller', buttons=[button], milliseconds=80)
    time.sleep(.2)
    return api('chooser')

def screenshot(name):
    shutil.copy2(api('screenshot')['path'], OUT / (name + '.png'))

def run():
    end = time.monotonic() + 150
    while time.monotonic() < end:
        try:
            state = api('status')
            if state['ready']:
                break
            response = call(ROOT, 'startup.continue', {})
        except ApiFailure:
            pass
        time.sleep(.5)
    check(not state['board'], 'starts from main menu without replacing a board')
    api('debug.backgrounds')
    api('debug.start', index=0)
    wait('chooser', lambda s: s['ready'] and not s['zombiePhase'])
    state = move('DPadRight')  # Pairing may consume the first navigation input.
    if state['selection']['index'] == 1:
        state = move('DPadLeft')
    check(state['selection']['index'] == 0, 'controller selects first plant')
    for _ in range(7):
        state = move('DPadRight')
    check(state['selection']['index'] == 7, 'controller traverses first plant row')
    state = move('DPadRight')
    check(state['page'] == 1 and state['selection']['index'] == 49, 'plant right edge opens next page')
    state = move('DPadRight')
    check(state['selection']['index'] == 50, 'next page retains horizontal neighbors')
    api('controller', leftStick=dict(x=-1, y=0), milliseconds=80)
    time.sleep(.2)
    check(api('chooser')['selection']['index'] == 49, 'left stick navigates fusion cards')
    state = move('DPadRight')
    move('DPadLeft')
    state = move('DPadLeft')
    check(state['page'] == 0 and state['selection']['index'] == 7, 'plant left edge returns to previous row end')
    for page in range(1, state['lastPage'] + 1):
        state = move('DPadRight')
        check(state['page'] == page, f'controller opens plant page {page}')
        if page == 1:
            screenshot('fusion-packets')
        if page < state['lastPage']:
            for _ in range(7):
                state = move('DPadRight')
    first = state['selection']['index']
    last = max(c['index'] for c in state['choices'])
    rows = (last - first) // 8
    for _ in range(rows):
        state = move('DPadDown')
    check(state['selection']['index'] == first + rows * 8, 'controller reaches incomplete last plant row')
    state = move('DPadLeft')
    check(state['page'] == state['lastPage'] - 1 and state['selection']['index'] == first - 48 + rows * 8 + 7, 'incomplete row returns to matching previous row end')
    state = move('DPadRight')
    check(state['page'] == state['lastPage'] and state['selection']['index'] == first + rows * 8, 'incomplete row accepts forward navigation')
    for _ in range(rows):
        state = move('DPadUp')
    for _ in range(7):
        state = move('DPadRight')
    state = move('DPadRight')
    check(state['page'] == state['lastPage'], 'last plant page does not overflow')
    screenshot('last-plant-page')
    # Use the native submit action on an actual fusion card.
    move('DPadLeft')
    selected = api('chooser')['selection']['index']
    api('controller', buttons=['A'], milliseconds=80)
    time.sleep(.3)
    state = api('chooser')
    check(any(c['index'] == selected and c['state'] == 'SeedInBank' for c in state['choices']), 'controller submits fusion card')
    api('chooser.select', indices=[selected], confirm=True)
    wait('chooser', lambda s: s['ready'] and s['zombiePhase'])
    state = api('chooser')
    if state['selection']['index'] is None:
        state = move('DPadRight')
    check(state['selection']['index'] == 0, 'controller selects first zombie')
    for _ in range(7):
        state = move('DPadRight')
    check(state['selection']['index'] == 7, 'controller traverses first zombie row')
    if state['lastPage'] > 0:
        state = move('DPadRight')
        check(state['page'] == 1 and state['selection']['index'] == 48, 'zombie right edge opens next page')
        state = move('DPadLeft')
        check(state['page'] == 0 and state['selection']['index'] == 7, 'zombie left edge returns to previous row end')
    else:
        state = move('DPadRight')
        check(state['page'] == 0, 'single zombie page does not overflow')
        state = move('DPadLeft')
        check(state['selection']['index'] == 7, 'controller returns from native side button to zombie grid')
    api('controller', buttons=['A'], milliseconds=80)
    time.sleep(.3)
    state = api('chooser')
    check(any(c['index'] == 7 and c['state'] == 'SeedInBank' for c in state['choices']), 'controller submits zombie card')
    screenshot('zombie-controller')
    api('chooser.select', indices=[], confirm=True)
    wait('status', lambda s: s['scene'] == 'Playing')
    check(True, 'both selections complete into temporary debug gameplay')

if __name__ == '__main__':
    try:
        run()
    finally:
        try:
            api('controller.reset')
        finally:
            (OUT / 'controller-runtime.json').write_text(json.dumps(dict(checks=checks, calls=events), indent=2) + '\n')
