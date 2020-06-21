#!/bin/bash
# Local compile, without msbuild
mcs /reference:bin/Debug/'Priority Queue.dll' -recurse:'*.cs'

# Proper compile
# msbuild file.sln