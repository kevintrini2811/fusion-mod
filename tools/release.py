"""Bump, check, build, back up and atomically install FusionMod without starting PvZ."""
import argparse
import fcntl
import json
import os
from pathlib import Path
import re
import subprocess
import tempfile

import pefile
from fusion_checks import check, version
from source_index import PROJECT, atomic_json, sha256

GAME = Path('/mnt/FastGaming1/SteamLibrary/steamapps/common/PVZ Replanted')


def dll_version(path):
    pe = pefile.PE(str(path), fast_load=False)
    try:
        info = pe.VS_FIXEDFILEINFO[0]
        parts = [info.FileVersionMS >> 16, info.FileVersionMS & 65535,
                 info.FileVersionLS >> 16, info.FileVersionLS & 65535]
        if parts[3] != 0:
            raise ValueError(f'Unsupported four-part DLL version: {parts}')
        return '.'.join(map(str, parts[:3]))
    finally:
        pe.close()


def atomic_bytes(path, content):
    fd, temporary = tempfile.mkstemp(dir=path.parent, prefix='.FusionMod-')
    try:
        with os.fdopen(fd, 'wb') as stream:
            stream.write(content)
            stream.flush()
            os.fsync(stream.fileno())
        os.chmod(temporary, 0o644)
        os.replace(temporary, path)
        directory = os.open(path.parent, os.O_DIRECTORY)
        try:
            os.fsync(directory)
        finally:
            os.close(directory)
    finally:
        if os.path.exists(temporary):
            os.unlink(temporary)


def release(game, requested):
    previous = version()
    parts = list(map(int, previous.split('.')))
    parts[-1] += 1
    target = requested or '.'.join(map(str, parts))
    if not re.fullmatch(r'\d+\.\d+\.\d+', target) or tuple(map(int, target.split('.'))) <= tuple(map(int, previous.split('.'))):
        raise ValueError('New version must be a higher major.minor.patch version')
    if not check():
        raise ValueError('Static checks failed before version bump')
    paths = [PROJECT/'FusionMod.csproj', PROJECT/'src/FusionMod.cs', PROJECT/'README.md']
    originals = {p: p.read_bytes() for p in paths}
    checksum_path = PROJECT/'SHA256SUMS'
    old_checksums = checksum_path.read_bytes()
    installed = game/'Mods/FusionMod.dll'
    old_dll = installed.read_bytes()
    library_installed = game/'Mods/PlantLibrary.dll'
    old_library = library_installed.read_bytes() if library_installed.exists() else None
    zombie_installed = game/'Mods/ZombieLibrary.dll'
    old_zombie = zombie_installed.read_bytes() if zombie_installed.exists() else None
    old_hash = sha256(installed)
    old_version = dll_version(installed)
    if old_version != previous:
        raise ValueError(f'Installed version {old_version} differs from project {previous}')
    backup = PROJECT/f'backup/FusionMod-{old_version}.dll'
    if backup.exists() and backup.read_bytes() != old_dll:
        raise ValueError(f'Backup already exists with different content: {backup}')
    deployed = False
    try:
        paths[0].write_text(paths[0].read_text().replace(f'<Version>{previous}</Version>', f'<Version>{target}</Version>'))
        paths[1].write_text(paths[1].read_text().replace(f'"Plant Fusion", "{previous}"', f'"Plant Fusion", "{target}"'))
        text = paths[2].read_text()
        paths[2].write_text(text.replace(f'# Fusion-Mod {previous}\n', f'# Fusion-Mod {target}\n', 1))
        if not check():
            raise ValueError('Static checks failed after version bump')
        dotnet = PROJECT.parent/'.tools/dotnet/dotnet'
        subprocess.run([str(dotnet), 'build', 'FusionMod.csproj', '-c', 'Release'], cwd=PROJECT, check=True)
        built = PROJECT/'bin/Release/net6.0/FusionMod.dll'
        if dll_version(built) != target:
            raise ValueError('Built DLL version mismatch')
        if sha256(installed) != old_hash:
            raise ValueError('Installed DLL changed during release; refusing to overwrite')
        backup.parent.mkdir(exist_ok=True)
        if not backup.exists():
            with backup.open('xb') as stream:
                stream.write(old_dll)
                stream.flush()
                os.fsync(stream.fileno())
        if sha256(backup) != old_hash:
            raise ValueError('Backup verification failed')
        library_built = PROJECT.parent/'plant-library/bin/Release/net6.0/PlantLibrary.dll'
        library_payload = library_built.read_bytes()
        if (library_installed.read_bytes() if library_installed.exists() else None) != old_library:
            raise ValueError('Installed PlantLibrary changed during release; refusing to overwrite')
        if old_library is not None:
            library_backup = PROJECT/f'backup/PlantLibrary-before-FusionMod-{old_version}.dll'
            if library_backup.exists() and library_backup.read_bytes() != old_library:
                raise ValueError('PlantLibrary backup exists with different content')
            atomic_bytes(library_backup, old_library)
        zombie_built = PROJECT.parent/'zombie-library/bin/Release/net6.0/ZombieLibrary.dll'
        zombie_payload = zombie_built.read_bytes()
        if (zombie_installed.read_bytes() if zombie_installed.exists() else None) != old_zombie:
            raise ValueError('Installed ZombieLibrary changed during release; refusing to overwrite')
        if old_zombie is not None:
            zombie_backup = PROJECT/f'backup/ZombieLibrary-before-FusionMod-{old_version}.dll'
            if zombie_backup.exists() and zombie_backup.read_bytes() != old_zombie:
                raise ValueError('ZombieLibrary backup exists with different content')
            atomic_bytes(zombie_backup, old_zombie)
        deployed = True
        atomic_bytes(zombie_installed, zombie_payload)
        atomic_bytes(library_installed, library_payload)
        atomic_bytes(installed, built.read_bytes())
        if built.read_bytes() != installed.read_bytes() or dll_version(installed) != target:
            raise ValueError('Installation binary/version mismatch')
        checksums = ''.join(f'{sha256(game/"Mods"/name)}  {name}\n' for name in ('LawnGlove.dll', 'PlantLibrary.dll', 'ZombieLibrary.dll', 'FusionMod.dll'))
        atomic_bytes(checksum_path, checksums.encode())
        proof = dict(version=target, previous_version=old_version, backup=str(backup),
                     installed=str(installed), built_sha256=sha256(built), installed_sha256=sha256(installed),
                     backup_sha256=sha256(backup), game_started=False, runtime_tests=False,
                     library_built_sha256=sha256(library_built), library_installed_sha256=sha256(library_installed),
                     zombie_built_sha256=sha256(zombie_built), zombie_installed_sha256=sha256(zombie_installed),
                     static_checks=True, assets={str(p.relative_to(PROJECT)):sha256(p)
                     for p in sorted((PROJECT/'assets').glob('*/*')) if p.is_file()})
        atomic_json(PROJECT/f'references/release-{target}.json', proof)
        print(f'Released {target}; backup {backup.name}; DLL binary/version verified; SHA256SUMS updated')
    except BaseException:
        if deployed:
            if old_zombie is None:
                zombie_installed.unlink(missing_ok=True)
            else:
                atomic_bytes(zombie_installed, old_zombie)
            if old_library is None:
                library_installed.unlink(missing_ok=True)
            else:
                atomic_bytes(library_installed, old_library)
            atomic_bytes(installed, old_dll)
            atomic_bytes(checksum_path, old_checksums)
        for path, content in originals.items():
            atomic_bytes(path, content)
        raise


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--version', help='Default: increment patch version')
    parser.add_argument('--game-dir', type=Path, default=GAME)
    args = parser.parse_args()
    with (args.game_dir/'Mods/.FusionMod.release.lock').open('a') as lock:
        fcntl.flock(lock, fcntl.LOCK_EX | fcntl.LOCK_NB)
        release(args.game_dir, args.version)
