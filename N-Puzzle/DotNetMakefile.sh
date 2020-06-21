#!/bin/bash

helpFunction()
{
   echo ""
   echo "Usage: $0 [options]"
   echo "\t-c : clean solution"
   echo "\t-b : build solution"
   echo "\t-r : rebuild solution"
   exit 1 # Exit script after printing help
}

while getopts "c:b:r:" opts
do
   case $opts in
      c ) parameterA=$OPTARG ;;
      b ) parameterB=$OPTARG ;;
      r ) parameterC=$OPTARG ;;
      ? ) helpFunction ;; # Print helpFunction in case parameter is non-existent
   esac
done

# Print helpFunction in case parameters are empty
if [ -z "$parameterA" ] && [ -z "$parameterB" ] && [ -z "$parameterC" ]
then
   # echo "Some or all of the parameters are empty";
   helpFunction
fi

if [ -z "$parameterA" ]
then
   :
else
    echo "---------------------------------------\n\t\e[91mClean\e[0m \e[92m$parameterA\e[0m\n---------------------------------------"
    dotnet clean $parameterA
fi

if [ -z "$parameterB" ]
then
   :
else
    echo "---------------------------------------\n\t\e[91mBuild\e[0m \e[92m$parameterB\e[0m\n---------------------------------------"
    msbuild $parameterB
fi

if [ -z "$parameterC" ]
then
    :
else
    echo "---------------------------------------\n\t\e[91mRebuild\e[0m \e[92m$parameterC\e[0m\n---------------------------------------"
    dotnet clean $parameterC
    msbuild $parameterC
fi

file="./N-Puzzle/bin/Debug/N-Puzzle.exe"
if [ -f "$file" ] && [ -z "$parameterA" ]
then
	chmod 777 "$file"
elif [ -z "$parameterA" ]
then
    echo "\e[91m$file\e[0m not found."
fi