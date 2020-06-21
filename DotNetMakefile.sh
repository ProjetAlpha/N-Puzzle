#!/bin/bash

helpFunction()
{
   echo ""
   echo -e "Usage: $0 [options]"
   echo -e "\t-c : clean solution"
   echo -e "\t-b : build solution"
   echo -e "\t-r : rebuild solution"
   echo -e "\t-p : build platform configuration"
   exit 1 # Exit script after printing help
}

while getopts "c:b:r:p:" opts
do
   case $opts in
      c ) parameterA=$OPTARG ;;
      b ) parameterB=$OPTARG ;;
      r ) parameterC=$OPTARG ;;
      p ) configuration=$OPTARG ;;
      ? ) helpFunction ;; # Print helpFunction in case parameter is non-existent
   esac
done

buildOption="-p:Configuration=Debug"

file="./N-Puzzle/bin/Debug/N-Puzzle.exe"
if [ "$configuration" = "Release" ]
then
   buildOption="-p:Configuration=Release"
   # file="./N-Puzzle/bin/Release/N-Puzzle.exe"
fi

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
    echo -e "---------------------------------------\n\t\e[91mClean\e[0m \e[92m$parameterA\e[0m\n---------------------------------------"
    msbuild -t:ReBuild $buildOption $parameterC
fi

if [ -z "$parameterB" ]
then
   :
else
    echo -e "---------------------------------------\n\t\e[91mBuild\e[0m \e[92m$parameterB\e[0m\n---------------------------------------"
    msbuild $buildOption $parameterB
fi

if [ -z "$parameterC" ]
then
    :
else
    echo -e "---------------------------------------\n\t\e[91mRebuild\e[0m \e[92m$parameterC\e[0m\n---------------------------------------"
    msbuild -t:Clean $parameterC
fi

if [ -f "$file" ] && [ -z "$parameterA" ]
then
	chmod 777 "$file"
elif [ -z "$parameterA" ]
then
    echo -e "\e[91m$file\e[0m not found."
fi