@echo off
powershell -NoProfile -ExecutionPolicy unrestricted -Command "& '%~dp0\buildframework\build.ps1' %*"
