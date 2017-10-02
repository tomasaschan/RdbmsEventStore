# RdbmsEventStore

A simple EventStore that sits on top of your existing relational database

[![Build status](https://ci.appveyor.com/api/projects/status/suiwieeiqjw7ljpa/branch/master?svg=true)](https://ci.appveyor.com/project/tlycken/rdbmseventstore) [![NuGet package](https://img.shields.io/nuget/v/RdbmsEventStore.svg)](https://nuget.org/packages/RdbmsEventStore)

## Why RdbmsEventStore?

You want to use [event sourcing](https://martinfowler.com/eaaDev/EventSourcing.html), but you don't want to (or aren't allowed to) host a completely new system in your production environment. You have a relational database already, so why not use that?

RdbmsEventStore is intended to provide some basic functionality for [event sourcing](https://martinfowler.com/eaaDev/EventSourcing.html) on top of a relational database. This is, naturally, going to be less optimal for event sourcing than using a custom-built event store like [Event Store](https://eventstore.org/), or even a document database, so if you're at liberty to choose such a product, it might serve you better.

On the other hand, if - for some reason - you need to build your event store on your existing stack, with a relational database and a .NET application, this package is for you. This project sprung out of just such a situation.

## State of the project

**This project is still in early beta stage.**

It sprung out of just such a situation - if I wanted an event store, I had to build it on top of Microsoft SQL Server. As such, I'm using it in code that is (or will soon be) in production - but I don't recommend anyone to do the same (yet) unless you've read and understood all the code in the project. If you have, Pull Requests are warmly welcome :)

## Show me some code!

All documentation is in the [project wiki](https://github.com/tlycken/RdbmsEventStore/wiki).