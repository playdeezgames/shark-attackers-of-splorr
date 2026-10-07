# Questionable quirks of the original

Found while porting. Not fixed in the exact port; decide after it ships.

- `ChooseNamePrompt` defines `DEFAULT_NAME = "Olen Kyrpa"` but never uses it.
- `EmbarkActivity` stores `chosenPronouns` before it is assigned, so it is always null.
- Quit is disabled in the browser build (`IsQuittable` false), so the Main Menu shows a dead entry.
- `Home.razor` renders `gridCell.Text` literally (missing `@`); the grid is never populated anyway.
- Ground, Inventory and character menu entries exist in code but can never appear: disabled choices are filtered out and no items exist; sharks are only present during combat, which pre-empts the menu.
- `MetaphorDisplay` / `Title`: the jam link URL contains `-And-whimsy` with a capital A.
- `AdModel.Show` leaves an unused `avatar` local when the break ends.
- `World.Save` is never called, so neither front end ever saved. The Odin port adds saving on purpose (decision 3).
- Set Heading / Set Speed call Look without clearing messages, so old messages pile up above the new look.
- Heading and speed are stored unclamped and clamped on read; out-of-range input is silently accepted.
- `DonePrompt` (Ok leading to Abandon) is never used.
- Status prints only `Status:`.
- Unmooring removes the pier mooring too, so after leaving the pier the avatar can only return by mooring first.
