# Questionable quirks of the original

Found while porting. Not fixed in the exact port; decide after it ships.

- `ChooseNamePrompt` defines `DEFAULT_NAME = "Olen Kyrpa"` but never uses it.
- `EmbarkActivity` stores `chosenPronouns` before it is assigned, so it is always null.
- Quit is disabled in the browser build (`IsQuittable` false), so the Main Menu shows a dead entry.
- `Home.razor` renders `gridCell.Text` literally (missing `@`); the grid is never populated anyway.
- Ground and Inventory entries are always shown and always disabled: no items exist.
- `MetaphorDisplay` / `Title`: the jam link URL contains `-And-whimsy` with a capital A.
- `AdModel.Show` leaves an unused `avatar` local when the break ends.
