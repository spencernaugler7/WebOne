
# Test out a hypermedia driven application

## Todo

- Fetch templates from Static class instead of injected service that is setup now.
- Set postgres database for contacts.
- [X] Figure out how to clean up dependancy injection (how do I cleanly inject services from multiple files)

## Templating

Using the Fluid library

- [Library Used](https://github.com/sebastienros/fluid)
- [Short guide](https://deanebarker.net/tech/fluid/)

## Guide

How we build hypermedia driven applications

- [Guide](https://hypermedia.systems/book/contents/)
  - [Bookmark](https://hypermedia.systems/a-web-1-0-application/)

## Project

Want to build a contacts app.

Features

- List of all contacts
  - Contact image to the left
  - First and Last name in the Middle
- Create a new contact.
  - Fields
    - Name (Start with this)
      - First
      - Last
    - Picture
    - Phone Number
    - Email
    - Address
  - All should be validated.
  - More to add as a bonus
    - Address
    - Website
    - Relationship.
- Search contacts by name
  - Want to be able to fuzzy match like fzf
- Sync from cloud storage.

## Docs

to generate image via d2 run this command

```bash
d2 --watch --theme 200 arch.d2
```
