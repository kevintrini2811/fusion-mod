"""Synthetic regression cases for the adapter report. No game code is loaded."""
import contextlib
import io
import json
from pathlib import Path
import subprocess
import tempfile
import unittest
from unittest.mock import patch

import csharp_adapters as adapters


class AdapterChecks(unittest.TestCase):
    def setUp(self):
        temporary = tempfile.TemporaryDirectory()
        self.addCleanup(temporary.cleanup)
        self.root = Path(temporary.name).resolve()
        self.addCleanup(patch.stopall)
        patch.object(adapters, "PROJECT", self.root).start()
        (self.root / "tests").mkdir()
        self.projects = [self.root / "tests" / f"{name}Tests.csproj" for name in ("First", "Second")]
        for project in self.projects:
            project.touch()
        self.shared = self.root / "tests/SharedStubs.cs"
        self.shared.touch()

    def test_shared_stub_selects_every_consumer(self):
        with patch.object(adapters, "evaluate", return_value={self.shared}):
            report = adapters.affected_suites([self.shared], "dotnet")
        self.assertEqual([row["project"] for row in report],
                         ["tests/FirstTests.csproj", "tests/SecondTests.csproj"])

    def test_source_selects_only_its_consumers(self):
        with patch.object(adapters, "evaluate", side_effect=[{self.shared}, set()]):
            report = adapters.affected_suites([self.shared], "dotnet")
        self.assertEqual(len(report), 1)
        self.assertEqual(report[0]["affected_by"], ["tests/SharedStubs.cs"])

    def test_uncovered_input_is_an_error_even_with_covered_input(self):
        with patch.object(adapters, "evaluate", return_value={self.shared}):
            with self.assertRaisesRegex(ValueError, "No C# suite covers"):
                adapters.affected_suites([self.shared, self.root / "src/New.cs"], "dotnet")

    def test_evaluation_tracks_imports_and_directory_props(self):
        group = self.root / "tests/Adapters/Shared.projitems"
        props = self.root / "tests/Directory.Build.props"
        output = {"Items": {"Compile": [{"FullPath": str(self.shared),
                   "DefiningProjectFullPath": str(group)}]},
                  "Properties": {"DirectoryBuildPropsPath": str(props),
                                 "DirectoryBuildTargetsPath": ""}}
        with patch.object(adapters.subprocess, "run", return_value=
                          subprocess.CompletedProcess([], 0, json.dumps(output), "")) as run:
            inputs = adapters.evaluate(self.projects[0], "dotnet")
        self.assertTrue({self.shared, group, props, self.projects[0]} <= inputs)
        command = run.call_args.args[0]
        self.assertIn("-getItem:Compile", command)
        self.assertNotIn("build", command)
        self.assertNotIn("run", command)

    def test_duplicate_or_missing_compile_input_is_an_error(self):
        item = {"FullPath": str(self.shared), "DefiningProjectFullPath": str(self.projects[0])}
        for items in ([item, item], [{**item, "FullPath": str(self.root / "missing.cs")} ]):
            with self.subTest(items=items):
                output = {"Items": {"Compile": items}, "Properties": {}}
                with patch.object(adapters.subprocess, "run", return_value=
                                  subprocess.CompletedProcess([], 0, json.dumps(output), "")):
                    with self.assertRaises(ValueError):
                        adapters.evaluate(self.projects[0], "dotnet")

    def test_default_report_never_builds_or_runs_tests(self):
        suite = {"project": "tests/FirstTests.csproj", "affected_by": [], "inputs": [],
                 "run_command": "dotnet run --project tests/FirstTests.csproj -c Release"}
        with patch.object(adapters, "dotnet_path", return_value="dotnet"), \
                patch.object(adapters, "affected_suites", return_value=[suite]), \
                patch.object(adapters.subprocess, "run") as run, \
                contextlib.redirect_stdout(io.StringIO()):
            self.assertEqual(adapters.main([]), 0)
        run.assert_not_called()

    def test_build_mode_only_compiles(self):
        suite = {"project": "tests/FirstTests.csproj", "affected_by": [], "inputs": [],
                 "run_command": "manual only"}
        with patch.object(adapters, "dotnet_path", return_value="dotnet"), \
                patch.object(adapters, "affected_suites", return_value=[suite]), \
                patch.object(adapters.subprocess, "run") as run, \
                contextlib.redirect_stdout(io.StringIO()):
            self.assertEqual(adapters.main(["--build"]), 0)
        self.assertEqual(run.call_args.args[0],
                         ["dotnet", "build", suite["project"], "-c", "Release", "--nologo"])


if __name__ == "__main__":
    unittest.main()
