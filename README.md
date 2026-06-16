# Unity Content Pipeline Demo

Unity editor tooling and C# pipeline demo for turning designer-authored CSV content into validated game-ready JSON config.

This project is built as portfolio proof for tools, content pipeline, and gameplay systems roles. The focus is not a flashy game screen; the focus is the workflow a production team needs when content grows past what humans can safely hand-edit.

## What This Proves

This demo shows that I can:

- define a simple content schema for game data
- parse designer-authored CSV without external packages
- validate data quality and produce actionable errors
- generate runtime-friendly JSON config
- generate Unity `ScriptableObject` item assets
- wrap the pipeline in a Unity Editor window
- keep core logic testable outside the Unity editor
- document the workflow so designers, engineers, and reviewers can understand it quickly

## Demo Scenario

The sample pipeline imports an item catalog:

```csv
id,displayName,category,rarity,power,cost,iconKey,description,tags,enabled
iron_sword,Iron Sword,Weapon,Common,12,100,icon_sword_iron,Reliable starter sword,melee;starter,true
```

The pipeline validates the content and generates JSON:

```json
{
  "itemCount": 1,
  "items": [
    {
      "id": "iron_sword",
      "displayName": "Iron Sword",
      "category": "Weapon"
    }
  ]
}
```

The actual output includes rarity, power, cost, icon key, description, tags, and enabled state.

## Features

- CSV parser with quoted-field support
- required-column checks
- strongly typed item definitions
- rarity parsing
- boolean parsing for designer-friendly values like `yes/no` and `true/false`
- duplicate ID detection
- ID format validation
- numeric range validation
- warnings for weak content metadata
- JSON export with deterministic ordering
- ScriptableObject generation for Unity-native content workflows
- Unity Editor window under `Tools/Davon/Content Pipeline Demo`
- .NET validation harness for local checks outside Unity

## Project Structure

| Path | Purpose |
| --- | --- |
| `Runtime/ContentPipeline` | Pure C# parser, validator, exporter, and pipeline orchestration |
| `Runtime/Unity/ItemDefinitionAsset.cs` | Unity-native generated item asset type |
| `Editor/ContentPipelineWindow.cs` | Unity Editor workflow wrapper |
| `Editor/ItemScriptableObjectGenerator.cs` | Creates or updates generated item assets |
| `Samples~/ItemCatalog/items.csv` | Valid sample designer-authored content |
| `Samples~/ItemCatalog/items_with_errors.csv` | Broken sample data for validation demos |
| `Samples~/ItemCatalog/Generated/item_catalog.generated.json` | Example generated config |
| `Tools~/ValidationHarness` | .NET console validation harness. The tilde keeps it out of Unity's package import. |

## Running The Local Harness

From the repository root:

```powershell
dotnet run --project .\Tools~\ValidationHarness\ValidationHarness.csproj
```

Expected result:

```text
Valid sample: PASS
Invalid sample: PASS
Generated JSON contains expected content: PASS
```

## Using In Unity

This repository is structured as a Unity package.

1. Open a Unity project.
2. Add this package from disk through Package Manager, or copy the package folder into a Unity project.
3. Open `Tools > Davon > Content Pipeline Demo`.
4. Select a CSV file, choose an output folder, and run `Validate And Generate`.
5. Review diagnostics in the editor window.
6. Use the generated JSON and/or generated `ItemDefinitionAsset` assets in runtime systems or build pipelines.

## Validation Examples

The validator reports errors for issues that should block content generation:

- missing required fields
- duplicate item IDs
- invalid ID format
- invalid rarity values
- negative power or cost

It reports warnings for issues that may still be shippable but deserve attention:

- missing icon keys
- short descriptions
- duplicate tags

## Why This Matters In Production

Game teams often move fast through spreadsheets, live ops tables, tuning sheets, and designer-authored content. Without validation, small data mistakes become runtime bugs, broken UI, economy errors, or QA churn.

A useful content pipeline does more than import data. It gives creators fast feedback, blocks dangerous mistakes, and generates predictable config for the game.

## Next Improvements

- Add a preview table in the editor window.
- Add diff output so designers can see what changed between imports.
- Add severity filters and exportable validation reports.
- Add Play Mode tests around loading generated JSON.
- Add a short video showing the full designer workflow.

## Interview Topics This Project Supports

I can use this project to discuss:

- how I separate core pipeline logic from Unity editor UI
- how validation rules protect production teams from bad content data
- how to design actionable error messages
- how tools work can improve designer velocity
- how to grow a simple import pipeline into a larger live content workflow
