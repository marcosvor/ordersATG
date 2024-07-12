# Orders ATG

This repo is a implementation of the FIX protocol using both sides of the communication: One for order generation (OrderGenerator folder) and the other for order processing (OrderAccumulator folder). It also has a docker-compose file for local database and queue processing in case you want to test it as well. Both applications have unit tests for the business case presented.


System Setup
------------

* This project uses .NET 8.

* All scripts are in Powershell, and should work on both Windows and Unix-based platforms.

Build and Test
--------------

You can run both applications using the commands below on each folder:

* `dotnet build` - to build
* `dotnet test` - to run all unit and acceptance tests (which use NUnit)
