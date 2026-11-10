"""Report affected offline C# suites; optionally build, never execute them."""
import argparse
import json
from pathlib import Path
import shlex
import shutil
import subprocess
import sys

PROJECT = Path(__file__).resolve().parents[1]


def dotnet_path(requested=None):
    if requested:
        resolved = shutil.which(requested)
        if not resolved:
            raise ValueError(f"dotnet executable not found: {requested}")
        return str(Path(resolved).resolve())
    bundled = PROJECT.parent / ".tools/dotnet/dotnet"
    if bundled.is_file():
        return str(bundled)
    resolved = shutil.which("dotnet")
    if not resolved:
        raise ValueError("dotnet SDK required for MSBuild evaluation")
    return resolved


def evaluate(project, dotnet):
    # Let MSBuild resolve imports, conditions and item paths. No target is run.
    result = subprocess.run(
        [dotnet, "msbuild", str(project), "-nologo", "-property:Configuration=Release",
         "-getItem:Compile",
         "-getProperty:DirectoryBuildPropsPath,DirectoryBuildTargetsPath"],
        cwd=PROJECT, capture_output=True, text=True, check=True,
    )
    data = json.loads(result.stdout)
    files = [Path(item["FullPath"]).resolve() for item in data["Items"]["Compile"]]
    if not files:
        raise ValueError(f"No Compile items: {project}")
    if len(files) != len(set(files)):
        raise ValueError(f"Duplicate Compile items: {project}")
    for path in files:
        if not path.is_file():
            raise ValueError(f"Missing Compile input in {project.name}: {path}")
    inputs = set(files) | {project.resolve()}
    inputs.update(Path(item["DefiningProjectFullPath"]).resolve()
                  for item in data["Items"]["Compile"])
    inputs.update(Path(value).resolve() for value in data["Properties"].values() if value)
    return inputs


def label(path):
    try:
        return str(path.relative_to(PROJECT))
    except ValueError:
        return str(path)


def affected_suites(changed, dotnet):
    projects = sorted((PROJECT / "tests").glob("*Tests.csproj"))
    if not projects:
        raise ValueError("No offline C# projects found")
    changed = {Path(path).resolve() for path in changed}
    covered = set()
    selected = []
    for project in projects:
        inputs = evaluate(project, dotnet)
        matches = changed & inputs
        covered.update(matches)
        if not changed or matches:
            selected.append({
                "project": label(project),
                "affected_by": sorted(label(path) for path in matches),
                "inputs": sorted(label(path) for path in inputs),
                "run_command": shlex.join([dotnet, "run", "--project", label(project),
                                           "-c", "Release"]),
            })
    unknown = changed - covered
    if unknown:
        raise ValueError("No C# suite covers: " + ", ".join(sorted(label(p) for p in unknown)))
    return selected


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--changed", nargs="+", default=[], metavar="FILE",
                        help="Changed sources, stubs, projects or shared build files; default: all suites")
    parser.add_argument("--dotnet", help="dotnet executable; default: bundled SDK, then PATH")
    parser.add_argument("--build", action="store_true", help="Compile selected suites without executing tests")
    parser.add_argument("--json", action="store_true", help="Print report as JSON; build output goes to stderr")
    args = parser.parse_args(argv)
    try:
        dotnet = dotnet_path(args.dotnet)
        # Resolve CLI paths against the caller's directory, before changing cwd.
        suites = affected_suites(args.changed, dotnet)
        if args.json:
            print(json.dumps(suites, indent=2, ensure_ascii=False), flush=True)
        else:
            print(f"{len(suites)} offline C# suite(s). Tests are not executed.")
            for suite in suites:
                reason = ", ".join(suite["affected_by"]) or "all suites"
                print(f"{suite['project']} ({reason})")
                print(f"  Run manually: {suite['run_command']}")
            sys.stdout.flush()
        if args.build:
            for suite in suites:
                subprocess.run([dotnet, "build", suite["project"], "-c", "Release", "--nologo"],
                               cwd=PROJECT, stdout=sys.stderr, stderr=sys.stderr, check=True)
        return 0
    except subprocess.CalledProcessError as error:
        print(f"MSBuild failed ({error.returncode}): {shlex.join(error.cmd)}", file=sys.stderr)
        if error.stdout:
            print(error.stdout.strip(), file=sys.stderr)
        if error.stderr:
            print(error.stderr.strip(), file=sys.stderr)
        return 1
    except (OSError, ValueError, KeyError, TypeError) as error:
        print(f"C# adapter report failed: {error}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
