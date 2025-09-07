#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2025 youtissoum <51883137+youtissoum@users.noreply.github.com>
#
# SPDX-License-Identifier: MIT

# A dictionary of file types with their comment character associated
FILE_TYPES: dict[str, str] = {
    ".cs": "//",
    ".yml": "#",
    ".py": "#",
}

# A dictionary associating the folder name of a namespace to their SPDX license identifier.
# Sorted alphabetically because it looks nice
FORK_NAMESPACES: dict[str, str] = {
    "":               "MIT", # upstream
    "_ADT":           "TODO", # I don't even know which server that is ??
    "_CD":            "MIT",
    "_DV":            "AGPL-3.0-or-later",
    "_EE":            "AGPL-3.0-or-later",
    "_EstacaoPirata": "AGPL-3.0-or-later",
    "_Goobstation":   "AGPL-3.0-or-later",
    "_Harmony":       "AGPL-3.0-or-later",
    "_Impstation":    "AGPL-3.0-or-later",
    "_LateStation":   "AGPL-3.0-or-later",
    "_NF":            "AGPL-3.0-or-later",
    "_RMC14":         "MIT",
    "_Umbra":         "MIT",
}

# A list of author names that will never be added to the copyright text
# Use only if an author cannot copyright (is a bot) or if someone has changed names.
IGNORED_AUTHORS: set[str] = []

# REUSE-IgnoreStart
SPDX_START = "SPDX"
SPDX_FILE_COPYRIGHT_TEXT = f"{SPDX_START}-FileCopyrightText"
SPDX_LICENSE_IDENTIFIER = f"{SPDX_START}-License-Identifier"
REUSE_IGNORE_START = "REUSE-IgnoreStart"
REUSE_IGNORE_END = "REUSE-IgnoreEnd"
# REUSE-IgnoreEnd

import argparse, os, subprocess, sys
from dataclasses import dataclass
from typing import Self

parser = argparse.ArgumentParser()
parser.add_argument("files", nargs="+", help="the list of files that need to be updated")
args = parser.parse_args()

files: list[str] = args.files

@dataclass
class FileSpdxData:
    copyrights: set[str]
    license_identifier: str|None
    extra_info: list[str]

def get_file_data(lines: list[str], comment_char: str) -> FileSpdxData:
    if len(lines) < 1:
        return FileSpdxData([], None, [])

    ignoring = False
    copyrights: set[str] = set()
    license_identifier: str|None = None
    extra_info: list[str] = []

    for line in lines:
        if line == "":
            break

        stripped_line = line.lstrip().lstrip(comment_char).lstrip()

        if stripped_line == REUSE_IGNORE_END:
            ignoring = False
            continue

        if stripped_line == REUSE_IGNORE_START:
            ignoring = True
            continue

        if ignoring:
            continue

        if not stripped_line.startswith(SPDX_START):
            continue

        if stripped_line.startswith(f"{SPDX_FILE_COPYRIGHT_TEXT}: "):
            copyrights.add(stripped_line[len(f"{SPDX_FILE_COPYRIGHT_TEXT}: "):])
            continue

        if stripped_line.startswith(f"{SPDX_LICENSE_IDENTIFIER}: "):
            identifier = stripped_line[len(f"{SPDX_LICENSE_IDENTIFIER}: "):]

            if license_identifier != None:
                print(f"Found duplicate license identifiers, overriding {license_identifier} with {identifier}")

            license_identifier = identifier
            continue

        extra_info.append(stripped_line)

    return FileSpdxData(copyrights, license_identifier, extra_info)

def clear_spdx_data(lines: list[str], comment_char: str):
    if len(lines) < 1:
        return FileSpdxData([], None, [])

    to_clear: list[int] = []
    last_was_spdx = False
    spdx_started = False
    ignoring = False

    for i, line in enumerate(lines):
        if line == "":
            # The things I do to support bad formatting
            if last_was_spdx:
                to_clear.append(i)
            break

        last_was_spdx = False

        stripped_line = line.lstrip().lstrip(comment_char).lstrip()

        if stripped_line == REUSE_IGNORE_END:
            ignoring = False
            continue

        if stripped_line == REUSE_IGNORE_START:
            ignoring = True
            continue

        if ignoring:
            continue

        if stripped_line == "" and spdx_started:
            to_clear.append(i)
            continue

        if stripped_line.startswith(SPDX_START):
            to_clear.append(i)
            spdx_started = True
            last_was_spdx = True
            continue

    for line in reversed(to_clear):
        lines.pop(line)

@dataclass
class CopyrightInfo:
    year: int|None
    name: str
    email: str|None

    # assumes the text actually follows the format :godo:
    def from_text(text: str) -> Self|None:
        parts: list[str] = text.split(' ')

        if len(parts) == 0:
            return None

        year: int|None = None
        name: str = ""
        email: str|None = None

        if parts[0].isnumeric():
            year = int(parts[0])
            parts.pop(0)

        for part in parts:
            if part.startswith('<'):
                email = part.lstrip('<').rstrip('>')
                break

            name += f"{part} "

        return CopyrightInfo(year, name[:-1], email)

    def to_text(self) -> str:
        output = ""

        if self.year != None:
            output += str(self.year) + " "

        output += self.name

        if self.email != None:
            output += f" <{self.email}>"

        return output

    def try_add_into(self, copyrights: list[Self]):
        if self.name in IGNORED_AUTHORS:
            return

        to_replace: int|None = None
        do_replace = False

        for i, author in enumerate(copyrights):
            if author.name != self.name:
                continue
            to_replace = i

            if (author.email == None) and (self.email != None):
                do_replace = True
                break

            if (author.year == None) and (self.year != None):
                if (self.email == None) and (author.email != None):
                    self.email = author.email
                do_replace = True
                break

            if (author.year != None) and (self.year != None):
                if author.year < self.year:
                    if (self.email == None) and (author.email != None):
                        self.email = author.email
                    do_replace = True
                    break

            break

        if to_replace == None:
            copyrights.append(self)
        else:
            if do_replace == True:
                copyrights.pop(to_replace)
                copyrights.append(self)

def get_copyright_text(existing_copyrights: list[str], file_name: str) -> list[CopyrightInfo]:
    new_copyrights: list[CopyrightInfo] = []

    log_process = subprocess.run(["git", "log", '--pretty=format:%as %aN <%aE>', file_name], capture_output=True)

    if log_process.returncode == 0:
        for line_bytes in log_process.stdout.splitlines():
            # This must break at least one law
            parts: list[str] = line_bytes.decode(encoding='utf-8').split(' ')
            parts[0] = parts[0].split('-')[0]

            CopyrightInfo.from_text(' '.join(parts)).try_add_into(new_copyrights)

    for author in existing_copyrights:
        copyright_info = CopyrightInfo.from_text(author)

        if copyright_info != None:
            copyright_info.try_add_into(new_copyrights)

    new_copyrights.sort(key=lambda x: x.name.lower())
    new_copyrights.sort(key=lambda x: (x.year or sys.maxsize))

    return new_copyrights

def update_file(file_name: str):
    print(f"Updating file {file_name}")

    try:
        with open(file_name, 'r') as f:
            data = f.read()
    except FileNotFoundError:
        print(f"Failed to find file {file_name}, skipping...")
        return

    lines = data.splitlines()
    file_type = os.path.splitext(file_name)[1]

    if file_type not in FILE_TYPES:
        print(f"Unsupported file type passed in: {file_type}, skipping...")
        return

    if len(lines) < 1:
        print(f"{file_name} is empty, skipping...")
        return

    shebang: str|None = None
    if lines[0].startswith("#!"):
        shebang = lines[0]
        lines.pop(0)

    while lines[0] == "":
        lines.pop(0)

    comment_char = FILE_TYPES[file_type]

    file_data = get_file_data(lines, comment_char)

    clear_spdx_data(lines, comment_char)

    new_data: list[str] = []

    if shebang != None:
        new_data.append(shebang)

    new_data += file_data.extra_info

    copyright_info = get_copyright_text(file_data.copyrights, file_name)

    for info in copyright_info:
        new_data.append(f"{comment_char} {SPDX_FILE_COPYRIGHT_TEXT}: {info.to_text()}")

    new_data += comment_char

    license_identifier = file_data.license_identifier
    license_identifier = None
    if license_identifier == None:
        for folder in reversed(file_name.replace('\\', '/').split('/')):
            if folder in FORK_NAMESPACES:
                license_identifier = FORK_NAMESPACES[folder]
                break

        # If it's still none, assume it's an upstream file
        if license_identifier == None:
            license_identifier = FORK_NAMESPACES[""]

    new_data.append(f"{comment_char} {SPDX_LICENSE_IDENTIFIER}: {license_identifier}")

    with open(file_name, 'w') as f:
        f.write('\n'.join(new_data))
        f.write('\n\n')
        f.write('\n'.join(lines))

for file in files:
    update_file(file)
