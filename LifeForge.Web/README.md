# LifeForge Web - Character Sheet

A Blazor WebAssembly front-end application for displaying and managing your LifeForge character.

## Overview

This app is loosely similar to Habitica, in that it is a motivational tool for managing your life
represented as an RPG character in a game.  There is no actual goal of "winning" the game, but rather
just a motivational overlay of constant improvement.

## Features

- **Buffs and Debuffs**: Track temporary effects on your character.  Buffs are user defined and
    customizable.
- **Actions**: Log actions that impact your character by providing buffs or debuffs.  These are
    generally manually triggered by the user.
- **Multiple Classes**: Support for various character classes with individual levels and XP. Each
    class has its own progression system of skills and/or abilities and/or feats.  These classes
    are defined and customized by the user.
- **Attributes**:
  - Strength represents physical strength. 
  - Discipline represents my ability to do what I set out to do.  Achieving tasks by their due dates
	increases Discipline. Not achieving tasks decreases Discipline.
	Forming habits increases discipline by a larger amount.
  - Focus represents ability to concentrate on tasks without getting distracted.
- **Quests**: Quests are special that provide unique rewards upon completion. Some quests have
    prerequisites such as reaching a certain level in a class or having a certain attribute value.
    Quests are user defined and customizable.
- **Tags**: Organize actions, quests, and other elements with user-defined tags to allow for custom
    behaviors such as actions that modify buffs of a certain tag.
- **Stats**
    - HP (Hit Points) bar with current/max values defining the progress bar.  Hit points are loosely
      a measure of current health, and HPMax is a measure of overall health max capacity.
    - MP (Mana Points) bar with current/max values defining the progress bar.  Mana points are loosely
      a measure of current mental energy, and MPMax is a measure of overall mental energy max capacity.
- **Wealth Tracking**: Display all currencies (Gold, Karma, Design Workslot, etc.)
  - Wealth is basically quantified by currencies.  Currencies can be earned and spent on various
    benefits in real life, such as spending virtual gold to buy yourself a nice meal at a local restaurant.\
    So therefore, wealth is used to trade for pleasurable experiences in real life.
  **XP** Tracking: XP is awarded primarily through completed quests.  XP is used to level up classes.
- **EOD Processing**: At the end of each day, the system processes daily updates such as applying buffs,
    updating modifiers from all active buffs/debuffs, and applying any other time-based changes to the character.

## Design Notes

- Uses Bootstrap for responsive layout
- Mobile-responsive design that adapts to smaller screens
- For now, this is only for personal use, so there is only 1 character supported.  However,
  I want to maintain a clean architecture that would allow for multiple characters in the future,
  like a full external website that other users can sign up for accounts and create their own characters.
- All external data storage is through MongoDB, accessed via the LifeForge API backend.
- Some features above are not yet implemented.  But are placeholders for future development.
