"""Export HelmetFume's original particle profile and sprite from Fusion 4.0.5.2."""
import json

import UnityPy
from export_snow_repeater import SourceAssets, SOURCE, PROJECT
from source_index import source_fingerprints, sha256


def export():
    fingerprints = source_fingerprints()
    bundle = SOURCE / "PlantsVsZombiesRH_Data/data.unity3d"
    environment = UnityPy.load(str(bundle))
    objects = {obj.path_id: obj for obj in environment.objects if obj.assets_file.name == "resources.assets"}
    source = SourceAssets(objects)
    root = source.read(58942)
    assert root["m_Name"] == "Fume_helmet"
    particle = next(objects[item["component"]["m_PathID"]] for item in root["m_Component"]
                    if objects[item["component"]["m_PathID"]].type.name == "ParticleSystem")
    data = source.read(particle.path_id)
    initial = data["InitialModule"]
    frames = [item["sprite"]["m_PathID"] for item in data["UVModule"]["sprites"]]
    assert frames == [12427]
    assert data["ShapeModule"]["enabled"] and data["ShapeModule"]["type"] == 18
    assert data["ColorModule"]["enabled"] and not data["SizeModule"]["enabled"]
    gradient = data["ColorModule"]["gradient"]["maxGradient"]
    assert gradient["m_NumAlphaKeys"] == 3 and gradient["m_NumColorKeys"] == 2
    assert all(gradient[f"key{i}"][channel] == 1 for i in (0, 1) for channel in "rgb")
    stops = [[gradient[f"atime{i}"] / 65535, 1, 1, 1, gradient[f"key{i}"]["a"]] for i in range(3)]
    speed = data["VelocityModule"]["speedModifier"]["maxCurve"]["m_Curve"]
    assert [(round(key["time"], 5), key["value"], key["inSlope"], key["outSlope"]) for key in speed] == [(.4, 1, 0, 0), (1, 0, 0, 0)]
    def limits(curve):
        assert curve["minMaxState"] == 3
        return [curve["minScalar"], curve["scalar"]]
    directory = PROJECT / "assets/fume-effects"
    model_path, profiles_path = directory / "model.json", directory / "profiles.json"
    model, profiles = json.loads(model_path.read_text()), json.loads(profiles_path.read_text())
    metadata, png, sprite_proof = source.sprite(frames[0])
    model["Sprites"][str(frames[0])] = metadata
    profiles["58942"] = dict(Frames=frames, Size=initial["startSize"]["scalar"],
        Life=limits(initial["startLifetime"]), Speed=limits(initial["startSpeed"]),
        Color=[initial["startColor"]["maxColor"][key] for key in "rgba"], Stops=stops,
        Spread=data["ShapeModule"]["m_Scale"]["y"], Emission=data["lengthInSec"],
        Rate=data["EmissionModule"]["rateOverTime"]["scalar"])
    (directory / metadata["File"]).write_bytes(png)
    model_path.write_text(json.dumps(model, separators=(",", ":")) + "\n")
    profiles_path.write_text(json.dumps(profiles, separators=(",", ":")) + "\n")
    proof_path = PROJECT / "references/football-shroom/smoke-export.json"
    proof_path.write_text(json.dumps(dict(bundle=str(bundle), bundle_sha256=fingerprints[str(bundle)],
        prefab=root, particle_path_id=particle.path_id, particle=data, sprite=sprite_proof), indent=2) + "\n")
    manifest_path = PROJECT / "references/fume-effects/visual-assets.json"
    manifest = json.loads(manifest_path.read_text())
    manifest["football_58942"] = dict(evidence=str(proof_path.relative_to(PROJECT)), sprite=sprite_proof)
    manifest["files"] = {file.name: sha256(file) for file in sorted(directory.iterdir()) if file.is_file()}
    manifest_path.write_text(json.dumps(manifest, separators=(",", ":")))
    print("Exported Fume_helmet 58942: original sprite12427, particle profile, source checksum and manifest.")


if __name__ == "__main__":
    export()
