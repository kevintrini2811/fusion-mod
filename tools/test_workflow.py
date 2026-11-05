"""Offline regression checks for workflow guards; never load or start the game."""
import contextlib
import io
import json
import re
from pathlib import Path
import shutil
import struct
import subprocess
import types
import tempfile
import unittest
from unittest.mock import patch

import fusion_checks
import almanac
import release
import source_index


class WorkflowChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        workspace = Path(self.temporary.name)
        self.project = workspace / "fusion-mod"
        self.project.mkdir()
        (workspace / "zombie-library").symlink_to(source_index.PROJECT.parent / "zombie-library", target_is_directory=True)
        for name in ('FusionMod.csproj', 'README.md', 'fusionen.txt', 'fusionen-quellen.json'):
            shutil.copyfile(source_index.PROJECT/name, self.project/name)
        shutil.copytree(source_index.PROJECT/'src', self.project/'src')
        (self.project/'tools').mkdir()
        # Release proofs include exporter scripts. Preserve those inputs too.
        for pattern in ('*.py', '*.json'):
            for source in (source_index.PROJECT/'tools').glob(pattern):
                shutil.copyfile(source, self.project/'tools'/source.name)
        for name in ('assets', 'references'):
            (self.project/name).symlink_to(source_index.PROJECT/name, target_is_directory=True)
        self.patch = patch.object(fusion_checks, 'PROJECT', self.project)
        self.patch.start()
        self.addCleanup(self.patch.stop)

    def checked(self):
        with contextlib.redirect_stdout(io.StringIO()):
            return fusion_checks.check()

    def test_real_project_passes(self):
        self.assertTrue(self.checked())

    def test_native_spread_recipes_keep_source_names_without_new_catalog_plants(self):
        source = fusion_checks.merged_recipe_source()
        rows = fusion_checks.recipes()
        names, implemented, remaining, errors = fusion_checks.recipe_state(source, rows, fusion_checks.availability())
        self.assertFalse(errors)
        for (a, b, result), name in fusion_checks.TRANSIENT_RECIPES.items():
            self.assertIn((a, b, result), rows)
            self.assertIn((tuple(sorted((names[a], names[b]))), name), implemented)
            self.assertNotIn((tuple(sorted((names[a], names[b]))), name), remaining)
        catalog, _ = almanac.catalog_inputs(rows, names, source)
        self.assertNotIn("Fume-shroom", catalog)
        self.assertNotIn("Scaredy-shroom", catalog)

    def test_cherry_spreader_original_models_and_source_identity(self):
        project = source_index.PROJECT
        prefab = json.loads((project/'references/cherry-spreader/prefab.json').read_text())
        self.assertEqual(prefab['game_object'], 63447)
        self.assertIn('CherryThreePeater', [c.get('script_name') for c in prefab['components']])
        plant = json.loads((project/'assets/cherry-spreader/model.json').read_text())
        self.assertEqual(set(plant['Clips']), {'idle', 'head1', 'shoot1', 'head2', 'shoot2', 'head3', 'shoot3'})
        manifest = json.loads((project/'references/cherry-spreader/visual-assets.json').read_text())
        self.assertEqual(manifest['animator_controller_path_id'], 6512)
        self.assertEqual(len(manifest['animator_layers']), 4)
        events = [e for o in manifest['objects'] if o['type'] == 'AnimationClip'
                  for e in o['events'] if e['functionName'] == 'AnimShoot']
        self.assertTrue(events)
        self.assertTrue(all(abs(e['time'] - .5) < .00001 for e in events))
        charge = json.loads((project/'references/cherry-spreader-charge/prefab.json').read_text())
        self.assertEqual(charge['game_object'], 57035)
        self.assertIn('TimeBomb', [c.get('script_name') for c in charge['components']])
        bomb = json.loads((project/'assets/cherry-spreader-bomb-projectile/model.json').read_text())
        self.assertEqual(set(bomb['Clips']), {'idle', 'charge'})
        self.assertAlmostEqual(bomb['Clips']['charge']['Duration'], 1 / 6)

    def test_squash_original_clips_and_renderer_visibility(self):
        import zlib
        project = source_index.PROJECT
        original = json.loads((project/'references/squash-fusions/clip-4646.json').read_text())
        enabled, = [b for b in original['m_ClipBindingConstant']['genericBindings']
                    if b['typeID'] == 212 and b['attribute'] == zlib.crc32(b'm_Enabled')]
        self.assertEqual(enabled['isPPtrCurve'], 0)
        for slug in ('cherry-squash', 'spicy-squash', 'icy-squash', 'hypno-squash'):
            model = json.loads((project/'assets'/slug/'model.json').read_text())
            self.assertEqual(set(model['Clips']), {'idle', 'lookleft', 'lookright', 'jump', 'jumppop'})
            self.assertFalse(model['ShootAdditive'])
            visibility, = [b for b in model['Clips']['jump']['Bindings'] if b['Attribute'] == 6]
            node = model['Nodes'][visibility['Node']]
            self.assertEqual(zlib.crc32(node['Path'].encode()), enabled['path'])
            self.assertEqual(visibility['Node'], model['GroundNode'])
            self.assertEqual(len(visibility['Channels']), 1)
            states = {key['Coefficients'][3] for key in visibility['Channels'][0]}
            self.assertEqual(states, {0, 1})
            source_controller = json.loads((project/'references'/slug/'6477-AnimatorController.json').read_text())
            layer, = source_controller['m_Controller']['m_LayerArray']
            self.assertEqual(layer['data']['(int&)m_LayerBlendingMode'], 0)
            self.assertEqual(layer['data']['m_SkeletonMask']['data']['m_Data'], [])

    def test_solar_ball_root_motion_and_speed_are_preserved_in_source(self):
        import zlib
        project = source_index.PROJECT
        model = json.loads((project/'assets/solar-nut-ball/model.json').read_text())
        reference = project/'references/solar-nut-ball'
        animator = json.loads((reference/'126525-Animator.json').read_text())
        controller = json.loads((reference/'6566-AnimatorController.json').read_text())
        self.assertTrue(animator['m_ApplyRootMotion'])
        clip = model['Clips']['Round']
        self.assertEqual(clip['Duration'], 2)
        root, = [b for b in clip['Bindings'] if b['Node'] == 0 and b['Attribute'] == 1]
        keys = root['Channels'][0]
        def sample(time):
            key = max((k for k in keys if k['Time'] <= time), key=lambda k:k['Time'])
            a,b,c,d = key['Coefficients']
            dt = time-key['Time']
            return ((a*dt+b)*dt+c)*dt+d
        self.assertAlmostEqual(sample(0), 0)
        self.assertAlmostEqual(sample(1), 4)
        self.assertAlmostEqual(sample(1.999), 7.996, places=4)
        # The source position cannot be replayed modulo Duration on a native
        # projectile: that introduces an eight-unit backwards jump each loop.
        self.assertGreater(sample(1.999)-sample(.001), 7.9)
        for state in controller['m_Controller']['m_StateMachineArray'][0]['data']['m_StateConstantArray']:
            self.assertEqual(state['data']['m_SpeedParamID'], zlib.crc32(b'Speed'))

    def test_chompzilla_prefab_matches_source_identity(self):
        reference = source_index.PROJECT/'references/chompzilla-0-1-35'
        identity = json.loads((reference/'identity.json').read_text())
        prefab = json.loads((reference/'prefab.json').read_text())
        archive = json.loads((source_index.PROJECT/'fusionen-quellen.json').read_text())
        result_ids = {r['fusion_id'] for r in archive['recipes'] if r['fusion'] == 'Chompzilla'}
        source_type, = [c['field'] for c in identity['constants'] if c['value'] in result_ids]
        self.assertIn(source_type, [c.get('script_name') for c in prefab['components']])
        manifest = json.loads((source_index.PROJECT/'references/chompzilla/visual-assets.json').read_text())
        model = json.loads((source_index.PROJECT/'assets/chompzilla/model.json').read_text())
        self.assertEqual(manifest['prefab_path_id'], prefab['game_object'])
        self.assertEqual(model['Nodes'][0]['Name'], prefab['name'])

    def test_bucket_source_identity_and_exports(self):
        project = source_index.PROJECT
        proof = json.loads((project/'references/bucket/identity.json').read_text())
        self.assertEqual({c['field']: c['value'] for c in proof['constants']},
                         {'BucketPlant': 224, 'IronPea': 1020, 'IronNut': 1029})
        for slug, source_id, script in (('bucket', 224, 'BucketPlant'),
                                        ('buckshooter', 1020, 'IronPeaShooter'),
                                        ('buck-nut', 1029, 'WallNut')):
            reference = project/'references'/slug
            almanac = json.loads((reference/'almanac.json').read_text())
            prefab = json.loads((reference/'prefab.json').read_text())
            model = json.loads((project/'assets'/slug/'model.json').read_text())
            manifest = json.loads((reference/'visual-assets.json').read_text())
            self.assertEqual(almanac['seedType'], source_id)
            self.assertIn(script, [c.get('script_name') for c in prefab['components']])
            self.assertEqual(manifest['prefab_path_id'], prefab['game_object'])
            self.assertEqual(model['Nodes'][0]['Name'], prefab['name'])
        recipes = json.loads((project/'references/bucket/recipes.json').read_text())
        self.assertEqual(recipes, [])  # Base ingredient, not an invented fusion.

    def test_duplicate_save_identity_fails(self):
        path = self.project/'src/FusionPlant.cs'
        path.write_text(path.read_text().replace('SeedType.Chomper, SeedType.Cherrybomb', 'SeedType.Wallnut, SeedType.Cherrybomb'))
        self.assertFalse(self.checked())

    def test_stale_family_registration_fails(self):
        path = self.project/'src/FusionPlant.cs'
        path.write_text(path.read_text().replace('plant == SilverNut', 'plant == SolarNut', 1))
        self.assertFalse(self.checked())

    def test_stale_renderer_registration_fails(self):
        path = self.project/'src/ImportedPeaVisuals.cs'
        path.write_text(path.read_text().replace('new(FusionPlant.SilverNut, "SilverNut"),', ''))
        self.assertFalse(self.checked())

    def test_missing_resource_fails(self):
        path = self.project/'FusionMod.csproj'
        path.write_text(path.read_text().replace('assets/cherry-chomper/*', 'assets/missing/*'))
        self.assertFalse(self.checked())

    def test_empty_original_shadow_cannot_be_runtime_footprint(self):
        assets = self.project/'assets'
        assets.unlink()  # Replace only the temporary project's symlink.
        shutil.copytree(source_index.PROJECT/'assets', assets)
        path = assets/'solarweed/model.json'
        model = json.loads(path.read_text())
        self.assertGreaterEqual(model['Nodes'][model['GroundNode']]['Sprite'], 0)
        model['GroundNode'] = next(i for i,n in enumerate(model['Nodes']) if n['Path'] == 'Shadow')
        self.assertEqual(model['Nodes'][model['GroundNode']]['Sprite'], -1)
        path.write_text(json.dumps(model))
        with contextlib.redirect_stdout(io.StringIO()) as output:
            self.assertFalse(fusion_checks.check())
        self.assertIn('Original footprint has no sprite: FusionMod.Solarweed', output.getvalue())

    def test_version_mismatch_fails(self):
        path = self.project/'src/FusionMod.cs'
        path.write_text(path.read_text().replace('"Plant Fusion", "', '"Plant Fusion", "99.'))
        self.assertFalse(self.checked())

    def test_wrong_count_fails(self):
        path = self.project/'README.md'
        path.write_text(re.sub(r'\*\*\d+ offene Rezepte\*\*', '**999999 offene Rezepte**', path.read_text()))
        self.assertFalse(self.checked())

    def test_registration_is_not_availability(self):
        path = self.project/'tools/ingredient-availability.json'
        data = json.loads(path.read_text())
        data['ingredients']['Chomper']['available'] = False
        path.write_text(json.dumps(data))
        self.assertFalse(self.checked())
        with patch('builtins.print') as output:
            self.assertFalse(fusion_checks.report('Cherry Chomper', strict=True))
        self.assertTrue(any('BLOCKED: Chomper' in str(call) for call in output.call_args_list))

    def test_release_failure_restores_version_and_dll(self):
        game = self.project/'game'
        (game/'Mods').mkdir(parents=True)
        dll = game/'Mods/FusionMod.dll'
        dll.write_bytes(b'old DLL')
        (game/'Mods/LawnGlove.dll').write_bytes(b'unchanged glove')
        (self.project/'SHA256SUMS').write_text('old checksums\n')
        built = self.project/'bin/Release/net6.0/FusionMod.dll'
        built.parent.mkdir(parents=True)
        built.write_bytes(b'new DLL')
        before = {p:p.read_bytes() for p in (self.project/'FusionMod.csproj', self.project/'src/FusionMod.cs', self.project/'README.md')}
        current = fusion_checks.version()
        new = current.rsplit('.', 1)[0]+'.'+str(int(current.rsplit('.', 1)[1])+1)
        def fake_version(path):
            return new if Path(path).read_bytes() == b'new DLL' else current
        with patch.object(release, 'PROJECT', self.project), patch.object(release, 'dll_version', fake_version), \
             patch.object(release.subprocess, 'run'), patch.object(release, 'atomic_json', side_effect=OSError('proof write failed')), \
             contextlib.redirect_stdout(io.StringIO()):
            with self.assertRaisesRegex(OSError, 'proof write failed'):
                release.release(game, None)
        self.assertEqual(dll.read_bytes(), b'old DLL')
        self.assertEqual((self.project/'SHA256SUMS').read_text(), 'old checksums\n')
        for path, data in before.items():
            self.assertEqual(path.read_bytes(), data)
        self.assertEqual((self.project/f'backup/FusionMod-{current}.dll').read_bytes(), b'old DLL')


class AlmanacChecks(unittest.TestCase):
    def setUp(self):
        temporary = tempfile.TemporaryDirectory()
        self.addCleanup(temporary.cleanup)
        self.project = Path(temporary.name)
        self.output = self.project / 'assets/almanac'
        self.output.mkdir(parents=True)
        self.reference = self.project / 'references/fusion-almanac'
        self.reference.mkdir(parents=True)
        self.rows = [('SeedType.Peashooter', 'SeedType.Cherrybomb', 'FusionPlant.CherryShooter')]
        self.names = {'SeedType.Peashooter':'Peashooter', 'SeedType.Cherrybomb':'Cherry Bomb',
                      'FusionPlant.CherryShooter':'Cherryshooter'}
        self.source = {'recipes':[{'fusion':'Cherryshooter', 'fusion_id':1001}]}
        english = dict(Name='Cherryshooter', Info='Damage: 40', Flavor='Original flavor.', Notes='')
        german = dict(Name='Kirschkanone', Info='Schaden: 40', Flavor='Originaler Begleittext.', Notes='')
        self.entries = [dict(SourceId=1001, Identity='FusionPlant.CherryShooter', CanonicalName='Cherryshooter',
                             English=english | dict(Recipes=['Cherry Bomb + Peashooter']),
                             German=german | dict(Recipes=['Kirschbombe + Erbsenkanone']),
                             Recipes=['Cherry Bomb + Peashooter'], Portrait='1001.png')]
        (self.reference/'LawnStringsTranslate.en.json').write_text(json.dumps(dict(plants=[dict(
            seedType=1001, name='Cherryshooter', info=english['Info'], introduce=english['Flavor'])])))
        (self.reference/'adaptations.json').write_text('{}')
        (self.reference/'translations.de.json').write_text(json.dumps(dict(
            Ingredients={'Cherry Bomb':'Kirschbombe', 'Peashooter':'Erbsenkanone'},
            Entries={'1001':german | dict(EnglishSha256=almanac.text_hash(english))})))
        (self.output / '1001.png').write_bytes(b'original portrait bytes')
        self.write_catalog()

    def write_catalog(self):
        (self.output / 'catalog.json').write_text(json.dumps(self.entries))
        manifest = dict(inputs={}, portraits=[dict(source_id=e['SourceId']) for e in self.entries],
                        files={p.name:source_index.sha256(p) for p in self.output.iterdir()})
        (self.reference / 'export.json').write_text(json.dumps(manifest))

    def checked(self):
        return almanac.check_catalog(self.project, self.rows, self.names, self.source)

    def test_catalog_reads_the_requested_project(self):
        self.assertEqual(self.checked(), [])

    def test_missing_or_duplicate_results_fail_even_with_matching_checksums(self):
        for entries in ([], self.entries * 2):
            with self.subTest(entries=len(entries)):
                self.entries = entries
                self.write_catalog()
                self.assertIn('Almanac must contain every implemented result exactly once', self.checked())

    def test_unimplemented_recipe_fails_even_with_matching_checksums(self):
        self.entries[0]['Recipes'].append('Unported plant + Peashooter')
        self.write_catalog()
        self.assertIn('Almanac recipes differ from runtime: Cherryshooter', self.checked())

    def test_wrong_native_identity_fails_even_with_matching_checksums(self):
        self.entries[0]['Identity'] = 'SeedType.Peashooter'
        self.write_catalog()
        self.assertIn('Almanac identity differs from implemented result: Cherryshooter', self.checked())

    def test_changed_or_missing_portrait_fails(self):
        (self.output / '1001.png').write_bytes(b'changed')
        self.assertIn('Almanac export checksum mismatch', self.checked())
        (self.output / '1001.png').unlink()
        self.assertIn('Missing or invalid almanac portrait: Cherryshooter', self.checked())

    def test_chinese_display_text_fails_even_with_matching_checksums(self):
        self.entries[0]['English']['Info'] = 'Damage: 40 樱桃'
        self.write_catalog()
        self.assertIn('Missing or unnormalized almanac English Info: Cherryshooter', self.checked())

    def test_german_recipe_must_use_translated_ingredients(self):
        self.entries[0]['German']['Recipes'] = ['Cherry Bomb + Peashooter']
        self.write_catalog()
        self.assertIn('Almanac German recipes differ from runtime: Cherryshooter', self.checked())

    def test_missing_german_translation_fails(self):
        del self.entries[0]['German']
        self.write_catalog()
        self.assertTrue(any('Invalid fusion almanac' in error for error in self.checked()))

    def test_changed_english_requires_translation_review(self):
        source = self.reference/'LawnStringsTranslate.en.json'
        data = json.loads(source.read_text())
        data['plants'][0]['info'] = 'Damage: 41'
        source.write_text(json.dumps(data))
        self.assertTrue(any('German translation needs review' in error for error in self.checked()))


class DocumentationPatchChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        workspace = Path(self.temporary.name)
        self.project = workspace / "fusion-mod"
        self.project.mkdir()
        (workspace / "zombie-library").symlink_to(source_index.PROJECT.parent / "zombie-library", target_is_directory=True)
        (self.project / "src").mkdir()
        (self.project / "tools").mkdir()
        self.source = {
            "recipes": [dict(zip(("plant1", "plant2", "fusion"), row)) for row in (
                ("Alpha", "Beta", "Cherry"), ("Alpha", "Blocked", "Cherry"),
                ("Cherry", "Beta", "Future"), ("Beta", "Beta", "Beta"),
                ("Alpha", "Alpha", "Beta"))],
            "recipe_count": 5, "result_plant_count": 3,
        }
        for row in self.source["recipes"]:
            row["fusion_id"] = {"Cherry": 100, "Future": 101, "Beta": 1}[row["fusion"]]
        self.source_path = self.project / "fusionen-quellen.json"
        self.source_path.write_text(json.dumps(self.source))
        self.recipe_path = self.project / "src/FusionRecipes.cs"
        self.recipe_path.write_text(
            "[(SeedType.Beta, SeedType.Beta)] = SeedType.Beta,\n"
            "[(SeedType.Beta, SeedType.Alpha)] = FusionPlant.Cherry,\n")
        self.entries = {"ingredients": {
            name: dict(available=available, evidence="Explicit review", identity=identity)
            for name, available, identity in (("Alpha", True, "SeedType.Alpha"),
                                             ("Beta", True, "SeedType.Beta"),
                                             ("Cherry", True, "FusionPlant.Cherry"),
                                             ("Blocked", False, "SeedType.Blocked"),
                                             ("Future", False, "FusionPlant.Future"))}}
        self.availability_path = self.project / "tools/ingredient-availability.json"
        self.availability_path.write_text(json.dumps(self.entries))
        self.pending_path = self.project / "fusionen.txt"
        self.pending = ("Pflanze1 | Pflanze2 | Fusionname\n"
                        "Alpha | Alpha | Beta\n"
                        "Alpha | Beta | Cherry\n"
                        "Alpha | Blocked | Cherry\n"
                        "Cherry | Beta | Future\n")
        self.expected_pending = self.pending.replace("Alpha | Beta | Cherry\n", "")
        self.pending_path.write_text(self.pending)
        self.readme_path = self.project / "README.md"
        self.readme = ("# Fusion-Mod 1.7.3\n\n"
                       "## Änderung alt\n\nHistorie: **99 offene Rezepte**.\n\n"
                       "## Liste\n\n**99 offene Rezepte** mit **98 Ergebnis-Pflanzen**.\n"
                       "Beschreibung bleibt erhalten.\n\n"
                       "## Stand\n\nAktive Rezepte:\n\n```text\nBeta | Beta | Beta\n```\n\n"
                       "Verhalten bleibt erhalten.\n\n## Weitere Angaben\n\nNicht ändern.\n")
        self.expected_readme = self.readme.replace(
            "**99 offene Rezepte** mit **98 Ergebnis-Pflanzen**",
            "**3 offene Rezepte** mit **3 Ergebnis-Pflanzen**").replace(
                "```text\nBeta | Beta | Beta\n```",
                "```text\nBeta | Beta | Beta\nAlpha | Beta | Cherry\n```")
        self.readme_path.write_text(self.readme)
        self.project_patch = patch.object(fusion_checks, "PROJECT", self.project)
        self.project_patch.start()
        self.addCleanup(self.project_patch.stop)

    def snapshot(self):
        return {p: p.read_bytes() for p in self.project.rglob("*") if p.is_file()}

    def test_notes_keep_active_and_blocked_alternatives_without_claiming_capabilities(self):
        before = self.snapshot()
        with contextlib.redirect_stdout(io.StringIO()) as output:
            generated = fusion_checks.documentation_notes("cHeRrY")
        self.assertEqual(output.getvalue(), "")
        self.assertEqual(self.snapshot(), before)
        self.assertTrue(generated.startswith("# Cherry: Fähigkeiten und Grenzen\n"))
        self.assertIn("**Entwurf:", generated)
        self.assertIn("`100`. Kein Replanted-`SeedType`", generated)
        self.assertIn("`FusionPlant.Cherry`", generated)
        self.assertIn("`Alpha | Beta | Cherry`: aktiv registriert", generated)
        self.assertIn("`Alpha | Blocked | Cherry`: gesperrt; nicht verfügbare Zutaten: Blocked", generated)
        self.assertIn("## Fähigkeiten im Original", generated)
        self.assertIn("## Abweichungen und Grenzen", generated)
        self.assertIn("## Speicherung und Lebenszyklus", generated)
        self.assertIn(source_index.sha256(self.recipe_path), generated)

    def test_notes_do_not_confuse_available_ingredients_with_active_recipe(self):
        generated = fusion_checks.documentation_notes("Beta")
        self.assertIn("`Alpha | Alpha | Beta`: Zutaten verfügbar; Rezept noch nicht registriert", generated)
        self.assertIn("`Beta | Beta | Beta`: aktiv registriert", generated)

    def test_notes_do_not_infer_result_availability(self):
        del self.entries["ingredients"]["Future"]
        self.availability_path.write_text(json.dumps(self.entries))
        generated = fusion_checks.documentation_notes("Future")
        self.assertIn("Ergebnisidentität: nicht als verfügbar freigegeben", generated)
        self.assertIn("Kein Eintrag vorhanden", generated)

    def test_notes_reject_unknown_results_and_ambiguous_source_ids(self):
        with self.assertRaisesRegex(ValueError, "Unknown result"):
            fusion_checks.documentation_notes("Missing")
        for value in (999, True, "100", None):
            with self.subTest(value=value):
                self.source["recipes"][1]["fusion_id"] = value
                self.source_path.write_text(json.dumps(self.source))
                with self.assertRaisesRegex(ValueError, "Ambiguous source identity"):
                    fusion_checks.documentation_notes("Cherry")

    def test_notes_reject_inconsistent_registration(self):
        self.recipe_path.write_text("[(SeedType.Alpha, SeedType.Blocked)] = FusionPlant.Cherry,\n")
        with self.assertRaisesRegex(ValueError, "Unverified availability"):
            fusion_checks.documentation_notes("Cherry")

    def test_notes_reject_changed_inputs_without_output(self):
        read_bytes = Path.read_bytes
        reads = 0

        def changing(path):
            nonlocal reads
            data = read_bytes(path)
            if path == self.recipe_path:
                reads += 1
                if reads == 2:
                    return data + b"// concurrent edit\n"
            return data

        with patch.object(Path, "read_bytes", changing), contextlib.redirect_stdout(io.StringIO()) as output, \
             self.assertRaisesRegex(ValueError, "Inputs changed"):
            fusion_checks.documentation_notes("Cherry")
        self.assertEqual(output.getvalue(), "")

    def apply(self, generated):
        executable = shutil.which("patch")
        if executable is None:
            self.skipTest("GNU patch is needed only to verify applying the generated artifact")
        result = subprocess.run([executable, "--binary", "--batch", "--fuzz=0", "-p1"],
                                input=generated.encode("utf-8"), cwd=self.project,
                                stdout=subprocess.PIPE, stderr=subprocess.PIPE)
        self.assertEqual(result.returncode, 0, result.stdout.decode() + result.stderr.decode())

    def test_generation_is_read_only_and_emits_only_a_patch(self):
        before = self.snapshot()
        with contextlib.redirect_stdout(io.StringIO()) as output:
            generated = fusion_checks.documentation_patch()
        self.assertEqual(output.getvalue(), "")
        self.assertEqual(self.snapshot(), before)
        self.assertTrue(generated.startswith("--- a/fusionen.txt\n+++ b/fusionen.txt\n"))
        self.assertIn("--- a/README.md\n+++ b/README.md\n", generated)
        self.assertNotIn("a/src/", generated)
        self.assertNotIn("a/fusionen-quellen.json", generated)

    def test_patch_preserves_history_order_and_unimplemented_alternatives(self):
        original_source = self.source_path.read_bytes()
        original_availability = self.availability_path.read_bytes()
        self.apply(fusion_checks.documentation_patch())
        self.assertEqual(self.pending_path.read_text(), self.expected_pending)
        self.assertEqual(self.readme_path.read_text(), self.expected_readme)
        self.assertEqual(self.source_path.read_bytes(), original_source)
        self.assertEqual(self.availability_path.read_bytes(), original_availability)
        self.assertEqual(fusion_checks.documentation_patch(), "")

    def test_removed_registration_returns_to_pending(self):
        self.recipe_path.write_text(self.recipe_path.read_text().splitlines(keepends=True)[1])
        self.apply(fusion_checks.documentation_patch())
        self.assertIn("Beta | Beta | Beta\n", self.pending_path.read_text())
        status = self.readme_path.read_text().split("## Stand\n", 1)[1]
        self.assertNotIn("Beta | Beta | Beta", status)
        self.assertIn("Alpha | Beta | Cherry", status)

    def test_ordered_source_alternative_with_same_pair_stays_pending(self):
        self.source["recipes"].append(dict(plant1="Beta", plant2="Alpha", fusion="Future"))
        self.source["recipe_count"] += 1
        self.source_path.write_text(json.dumps(self.source))
        self.apply(fusion_checks.documentation_patch())
        self.assertIn("Beta | Alpha | Future\n", self.pending_path.read_text())
        self.assertNotIn("Alpha | Beta | Cherry\n", self.pending_path.read_text())

    def test_crlf_is_preserved_in_both_documents(self):
        self.pending_path.write_bytes(self.pending.replace("\n", "\r\n").encode())
        self.readme_path.write_bytes(self.readme.replace("\n", "\r\n").encode())
        self.apply(fusion_checks.documentation_patch())
        self.assertEqual(self.pending_path.read_bytes(), self.expected_pending.replace("\n", "\r\n").encode())
        self.assertEqual(self.readme_path.read_bytes(), self.expected_readme.replace("\n", "\r\n").encode())

    def test_missing_final_newline_produces_an_applicable_patch(self):
        self.pending_path.write_bytes(self.pending.rstrip("\n").encode())
        generated = fusion_checks.documentation_patch()
        self.assertIn("\\ No newline at end of file\n", generated)
        self.apply(generated)
        self.assertEqual(self.pending_path.read_text(), self.expected_pending)

    def test_availability_is_never_inferred_from_registration(self):
        del self.entries["ingredients"]["Cherry"]
        self.availability_path.write_text(json.dumps(self.entries))
        with self.assertRaisesRegex(ValueError, "Unverified availability"):
            fusion_checks.documentation_patch()

    def test_conflicting_identity_refuses_generation(self):
        self.entries["ingredients"]["Cherry"]["identity"] = "SeedType.Alpha"
        self.availability_path.write_text(json.dumps(self.entries))
        with self.assertRaisesRegex(ValueError, "Duplicate available identity"):
            fusion_checks.documentation_patch()

    def test_missing_availability_evidence_refuses_generation(self):
        self.entries["ingredients"]["Cherry"]["evidence"] = ""
        self.availability_path.write_text(json.dumps(self.entries))
        with self.assertRaisesRegex(ValueError, "evidence"):
            fusion_checks.documentation_patch()

    def test_unarchived_registration_refuses_generation(self):
        self.recipe_path.write_text("[(SeedType.Alpha, SeedType.Beta)] = SeedType.Beta,\n")
        with self.assertRaisesRegex(ValueError, "Recipe missing in source archive"):
            fusion_checks.documentation_patch()

    def test_duplicate_unordered_registrations_refuse_generation(self):
        self.recipe_path.write_text(self.recipe_path.read_text()
                                   + "[(SeedType.Alpha, SeedType.Beta)] = FusionPlant.Cherry,\n")
        with self.assertRaisesRegex(ValueError, "Duplicate unordered recipes"):
            fusion_checks.documentation_patch()

    def test_duplicate_archive_rows_refuse_generation(self):
        self.source["recipes"].append(dict(self.source["recipes"][0]))
        self.source["recipe_count"] += 1
        self.source_path.write_text(json.dumps(self.source))
        with self.assertRaisesRegex(ValueError, "Duplicate source recipes"):
            fusion_checks.documentation_patch()

    def test_incorrect_archive_counts_refuse_generation(self):
        self.source["recipe_count"] = 999
        self.source_path.write_text(json.dumps(self.source))
        with self.assertRaisesRegex(ValueError, "Archive recipe count mismatch"):
            fusion_checks.documentation_patch()

    def test_unknown_pending_row_refuses_generation(self):
        self.pending_path.write_text(self.pending + "Alpha | Unknown | Cherry\n")
        with self.assertRaisesRegex(ValueError, "absent from source archive"):
            fusion_checks.documentation_patch()

    def test_unrecognized_document_layout_refuses_generation(self):
        variants = (self.readme.replace("## Liste\n", "## Backlog\n"),
                    self.readme.replace("## Stand\n", "## Aktuell\n"),
                    self.readme + "\n## Liste\nDuplicate section.\n",
                    self.readme.replace("```text\n", "```\n"),
                    self.readme.replace("**98 Ergebnis-Pflanzen**", "unbekannt"),
                    self.readme.replace("**98 Ergebnis-Pflanzen**", "**98 Ergebnis-Pflanzen** **98 Ergebnis-Pflanzen**"),
                    self.readme.replace("Beta | Beta | Beta", "Beta | Beta | Beta\nBeta | Beta | Beta"),
                    self.readme.replace("Beta | Beta | Beta", "Not a recipe"))
        for text in variants:
            with self.subTest(text=text):
                self.readme_path.write_text(text)
                before = self.snapshot()
                with self.assertRaises(ValueError):
                    fusion_checks.documentation_patch()
                self.assertEqual(self.snapshot(), before)

    def test_changed_input_refuses_generation_without_output(self):
        read_bytes = Path.read_bytes
        reads = 0

        def changing(path):
            nonlocal reads
            data = read_bytes(path)
            if path == self.recipe_path:
                reads += 1
                if reads == 2:
                    return data + b"// concurrent edit\n"
            return data

        with patch.object(Path, "read_bytes", changing), contextlib.redirect_stdout(io.StringIO()) as output, \
             self.assertRaisesRegex(ValueError, "Inputs changed"):
            fusion_checks.documentation_patch()
        self.assertEqual(output.getvalue(), "")


class ResourceSyncChecks(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        self.project = Path(self.temporary.name) / "fusion-mod"
        (self.project / "src").mkdir(parents=True)
        self.path = self.project / "FusionMod.csproj"
        self.original = (
            '<Project Sdk="Microsoft.NET.Sdk">\n'
            '  <!-- Preserve this comment and all existing formatting. -->\n'
            '  <PropertyGroup><Version>1.7.3</Version></PropertyGroup>\n'
            '  <ItemGroup>\n'
            '    <Compile Include="src/**/*.cs" />\n'
            '    <EmbeddedResource Include="assets/alpha/*" '
            'LogicalName="FusionMod.Alpha.%(Filename)%(Extension)" />\n'
            '  </ItemGroup>\n'
            '</Project>\n').encode()
        self.path.write_bytes(self.original)
        self.path.chmod(0o640)
        self.visual = self.project / "src/ImportedPeaVisuals.cs"
        self.visual.write_text('new(FusionPlant.Alpha, "Alpha"),\n'
                               'new(\nFusionPlant.IceAlpha,\n"IceAlpha"),\n')
        for slug in ("alpha", "ice-alpha", "unused"):
            self.add_model(slug)
        self.project_patch = patch.object(fusion_checks, "PROJECT", self.project)
        self.project_patch.start()
        self.addCleanup(self.project_patch.stop)

    def add_model(self, slug):
        model = self.project / "assets" / slug / "model.json"
        model.parent.mkdir(parents=True)
        model.write_text("{}\n")
        return model

    def sync(self, write=False):
        with contextlib.redirect_stdout(io.StringIO()):
            return fusion_checks.sync_resources(write)

    def test_plan_is_read_only(self):
        with contextlib.redirect_stdout(io.StringIO()) as output:
            self.assertFalse(fusion_checks.sync_resources())
        self.assertIn('Include="assets/ice-alpha/*"', output.getvalue())
        self.assertNotIn('assets/unused/', output.getvalue())
        self.assertEqual(self.path.read_bytes(), self.original)

    def test_write_preserves_xml_permissions_and_is_idempotent(self):
        self.assertTrue(self.sync(write=True))
        addition = ('    <EmbeddedResource Include="assets/ice-alpha/*" '
                    'LogicalName="FusionMod.IceAlpha.%(Filename)%(Extension)" />\n').encode()
        expected = self.original.replace(b'  </ItemGroup>', addition + b'  </ItemGroup>')
        self.assertEqual(self.path.read_bytes(), expected)
        self.assertEqual(self.path.stat().st_mode & 0o777, 0o640)
        inode = self.path.stat().st_ino
        self.assertTrue(self.sync(write=True))
        self.assertEqual(self.path.read_bytes(), expected)
        self.assertEqual(self.path.stat().st_ino, inode)
        self.assertFalse(list(self.project.glob(".model-resources-*")))

    def test_crlf_line_endings_are_preserved(self):
        self.path.write_bytes(self.original.replace(b"\n", b"\r\n"))
        self.assertTrue(self.sync(write=True))
        self.assertNotIn(b"\n", self.path.read_bytes().replace(b"\r\n", b""))

    def test_missing_later_model_prevents_all_writes(self):
        self.visual.write_text(self.visual.read_text() + 'new(FusionPlant.ZMissing, "ZMissing"),\n')
        with self.assertRaisesRegex(ValueError, "Expected one exported model for ZMissing"):
            self.sync(write=True)
        self.assertEqual(self.path.read_bytes(), self.original)

    def test_ambiguous_folder_names_are_rejected(self):
        self.visual.write_text(self.visual.read_text() + 'new(FusionPlant.Model12, "Model12"),\n')
        self.add_model("model12")
        self.add_model("model-12")
        with self.assertRaisesRegex(ValueError, "Expected one exported model for Model12; found 2"):
            self.sync(write=True)
        self.assertEqual(self.path.read_bytes(), self.original)

    def test_conflicting_directory_registration_is_not_changed(self):
        conflicting = self.original.replace(b"FusionMod.Alpha.", b"FusionMod.IceAlpha.")
        self.path.write_bytes(conflicting)
        with self.assertRaisesRegex(ValueError, "already registered as IceAlpha"):
            self.sync(write=True)
        self.assertEqual(self.path.read_bytes(), conflicting)

    def test_duplicate_resource_names_are_rejected(self):
        duplicate = ('    <EmbeddedResource Include="assets/unused/*" '
                     'LogicalName="FusionMod.Alpha.%(Filename)%(Extension)" />\n').encode()
        invalid = self.original.replace(b"  </ItemGroup>", duplicate + b"  </ItemGroup>")
        self.path.write_bytes(invalid)
        with self.assertRaisesRegex(ValueError, "Duplicate model resource"):
            self.sync(write=True)
        self.assertEqual(self.path.read_bytes(), invalid)

    def test_conditional_resources_are_not_assumed_to_be_embedded(self):
        invalid = self.original.replace(b"<ItemGroup>", b'<ItemGroup Condition="true">')
        self.path.write_bytes(invalid)
        with self.assertRaisesRegex(ValueError, "unconditional"):
            self.sync(write=True)
        self.assertEqual(self.path.read_bytes(), invalid)

    def test_computed_renderer_names_are_rejected(self):
        self.visual.write_text(self.visual.read_text() + 'new(FusionPlant.Other, computedName),\n')
        with self.assertRaisesRegex(ValueError, "Unrecognized renderer model declaration"):
            self.sync(write=True)
        self.assertEqual(self.path.read_bytes(), self.original)

    def test_changed_project_is_not_overwritten(self):
        create_temporary = fusion_checks.tempfile.mkstemp
        changed = self.original + b"<!-- concurrent edit -->\n"

        def change_project(*args, **kwargs):
            result = create_temporary(*args, **kwargs)
            self.path.write_bytes(changed)
            return result

        with patch.object(fusion_checks.tempfile, "mkstemp", side_effect=change_project), \
             self.assertRaisesRegex(ValueError, "Project changed during resource sync"):
            self.sync(write=True)
        self.assertEqual(self.path.read_bytes(), changed)
        self.assertFalse(list(self.project.glob(".model-resources-*")))

    def test_replace_failure_keeps_original_project(self):
        with patch.object(fusion_checks.os, "replace", side_effect=OSError("replace failed")), \
             self.assertRaisesRegex(OSError, "replace failed"):
            self.sync(write=True)
        self.assertEqual(self.path.read_bytes(), self.original)
        self.assertFalse(list(self.project.glob(".model-resources-*")))

    def test_symlinked_project_is_not_replaced(self):
        target = self.project / "original.csproj"
        self.path.rename(target)
        self.path.symlink_to(target)
        with self.assertRaisesRegex(ValueError, "symlinked project"):
            self.sync(write=True)
        self.assertTrue(self.path.is_symlink())
        self.assertEqual(target.read_bytes(), self.original)

    def test_model_symlink_cannot_escape_assets(self):
        outside = self.project / "outside.json"
        outside.write_text("{}")
        model = self.project / "assets/ice-alpha/model.json"
        model.unlink()
        model.symlink_to(outside)
        with self.assertRaisesRegex(ValueError, "Model path escapes assets"):
            self.sync(write=True)
        self.assertEqual(self.path.read_bytes(), self.original)


class IndexChecks(unittest.TestCase):
    def test_checksum_invalidates_cache(self):
        with tempfile.TemporaryDirectory() as temporary:
            project = Path(temporary)
            source = project/'source'
            source.mkdir()
            bundle = source/'data.unity3d'
            bundle.write_bytes(b'first bundle')
            metadata = source/'metadata.dat'
            data = bytearray(168)
            struct.pack_into('<II', data, 0, 0xFAB11BAF, 31)
            metadata.write_bytes(data)
            dll = source/'GameAssembly.dll'
            dll.write_bytes(b'source DLL')
            (project/'fusionen-quellen.json').write_text(json.dumps(dict(game_assembly_sha256=source_index.sha256(dll))))
            environment = types.SimpleNamespace(objects=[])
            with patch.object(source_index, 'PROJECT', project), patch.object(source_index, 'SOURCE', source), \
                 patch.object(source_index, 'BUNDLE', bundle), patch.object(source_index, 'METADATA', metadata), \
                 patch.dict('sys.modules', UnityPy=types.SimpleNamespace(load=lambda _: environment)), \
                 contextlib.redirect_stdout(io.StringIO()):
                first = source_index.source_index(environment=environment)
                # Invalid environment proves the valid cache avoids object scanning.
                self.assertEqual(source_index.source_index(environment=object()), first)
                bundle.write_bytes(b'changed bundle, same location')
                second = source_index.source_index(environment=environment)
                self.assertNotEqual(first['sha256'], second['sha256'])
                loads = []

                def load_environment():
                    loads.append(True)
                    return environment

                self.assertEqual(source_index.source_index(environment=load_environment), second)
                self.assertEqual(loads, [])
                self.assertEqual(source_index.source_index(environment=load_environment, refresh=True), second)
                self.assertEqual(loads, [True])


if __name__ == '__main__':
    unittest.main()
