# SPDX-FileCopyrightText: 2025 youtissoum <51883137+youtissoum@users.noreply.github.com>
#
# SPDX-License-Identifier: AGPL-3.0-or-later

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

import argparse

parser = argparse.ArgumentParser()
parser.add_argument("files", nargs="+", help="the list of files that need to be updated")
args = parser.parse_args()

files: list[str] = args.files

print(files)
