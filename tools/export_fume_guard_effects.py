"""Export the two additional original smoke profiles for the four plant ports."""
import json
from pathlib import Path

import UnityPy
from export_snow_repeater import SourceAssets, SOURCE, PROJECT
from source_index import source_fingerprints, sha256


def export():
    fingerprints = source_fingerprints()
    bundle = SOURCE / "PlantsVsZombiesRH_Data/data.unity3d"
    environment = UnityPy.load(str(bundle))
    objects = {obj.path_id: obj for obj in environment.objects if obj.assets_file.name == "resources.assets"}
    source = SourceAssets(objects)
    directory = PROJECT / "assets/fume-effects"
    profiles = json.loads((directory / "profiles.json").read_text())
    sprites = json.loads((directory / "model.json").read_text())["Sprites"]
    proof = dict(bundle=str(bundle), sha256=fingerprints[str(bundle)], objects={})

    def limits(curve):
        return [curve["minScalar"], curve["scalar"]] if curve["minMaxState"] == 3 else [curve["scalar"]] * 2

    for identity in (31975, 45293):
        prefab = source.read(identity)
        components = [objects[item["component"]["m_PathID"]] for item in prefab["m_Component"]]
        particle = next(obj for obj in components if obj.type.name == "ParticleSystem")
        data = source.read(particle.path_id)
        initial = data["InitialModule"]
        frames = [item["sprite"]["m_PathID"] for item in data["UVModule"]["sprites"]]
        assert all(str(frame) in sprites for frame in frames), "Missing original smoke sprite"
        assert not data["ShapeModule"]["enabled"] and not data["ColorModule"]["enabled"]
        profiles[str(identity)] = dict(Frames=frames, Size=initial["startSize"]["scalar"],
            Life=limits(initial["startLifetime"]), Speed=limits(initial["startSpeed"]),
            Color=[initial["startColor"]["maxColor"][key] for key in ("r", "g", "b", "a")],
            Stops=[[0, 1, 1, 1, 1], [1, 1, 1, 1, 1]], ConstantSpeed=True, Emission=data["lengthInSec"], Rate=data["EmissionModule"]["rateOverTime"]["scalar"], Spread=0)
        proof["objects"][str(identity)] = dict(prefab=prefab, particle=data)
    (directory / "profiles.json").write_text(json.dumps(profiles, separators=(",", ":")) + "\n")
    (PROJECT / "references/fume-guard-21-2-0/fume-particles.json").write_text(json.dumps(proof, separators=(",", ":")))
    manifest = PROJECT / "references/fume-effects/visual-assets.json"
    data = json.loads(manifest.read_text())
    data["files"] = {file.name: sha256(file) for file in sorted(directory.iterdir()) if file.is_file()}
    manifest.write_text(json.dumps(data, separators=(",", ":")))
    print("Exported original Fume_cherry and Fume_zombie profiles; existing sprites reused.")


if __name__ == "__main__":
    export()
