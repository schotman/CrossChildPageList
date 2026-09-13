# Child Page List (CrossChildPageList)

A small DNN module that renders a list of child pages (tabs) for a chosen page. Point it at a parent page and it displays that page's children as links — optionally recursively, with icons, and in a configurable number of columns.

Originally a free module from DnnModule.com. This repository contains a maintenance update that restores compatibility with modern DNN and fixes a couple of long-standing bugs, updated by 40Fingers.

> **This is now a compiled module.** The code-behind is built into `CrossChildPageList.dll` by a Visual Studio 2026 / .NET Framework 4.7.2 (`net472`) SDK-style project, rather than being compiled at runtime by DNN's `BuildManager`. Only the `.ascx` markup, templates, images and resources are shipped as loose files; the C# ships as the assembly.

## Features

- Lists the child pages of any selected page (or the site's root menu).
- Optional **recursive** mode that walks the full descendant tree with indentation.
- Optional page **icons** next to each link.
- Configurable **columns per row**.
- Choice of link **target** (`_self` / `_blank`).
- Pluggable **display template** (the list markup lives in a user control you can swap).
- Respects **page view permissions** — users only see pages they are allowed to view.

## Compatibility

- **DNN 9.10.0 → 10.4.x** (verified against platform source).
- The module manifest declares a minimum core version of `09.10.00`, so DNN blocks installation on older builds.

> Note: on DNN 10.2.4+ the module uses one API (`TabController.GetPortalTabs`) that the platform has marked deprecated. It still functions; it only produces a deprecation warning in the logs. It was intentionally kept to preserve compatibility down to 9.10 (the non-deprecated replacement requires 10.2.4+).

## Installation

1. In DNN, go to **Settings → Extensions → Install Extension**.
2. Upload the module install package (the `.zip` from `install/`, containing `CrossChildPageList.dnn`, `CrossChildPageList.dll` and `Resources.zip`).
3. Follow the wizard to finish.
4. Add the **Child Page List** module to any page.

## Configuration

Open the module's **Settings** (pencil / *Child Page List Settings*) to configure:

| Setting | Description |
| --- | --- |
| **Parent Tab** | The page whose children are listed. Choose a specific page, or the "none specified" / root option to list top-level pages. Defaults to the current page. |
| **Include Self** | Also include the parent/current page in the list. |
| **Include Hidden Tab** | Include pages that are hidden from the menu (`Include In Menu` unchecked). |
| **Recursive** | Walk the entire descendant tree instead of only direct children. |
| **Display Icon** | Show each page's icon (falls back to a default icon when a page has none). |
| **List Template** | The user control used to render the list. Must be an `.ascx` inside the module's `Template` folder. |
| **Columns per row** | Number of columns in the rendered list (integer ≥ 1). |
| **Link Target** | `_self` (same window) or `_blank` (new window). |

Pages the current user is not authorized to view, deleted pages, and pages with link disabled are always excluded from the output.

## Templates

The list markup is a swappable user control. The default template ships at:

```
Template/default/List_Standard.ascx
```

To create a custom look, add another `.ascx` (with its `List.ascx.cs` code-behind class) under a subfolder of `Template/` and select it via the **List Template** setting. Because the module is compiled, a custom template's code-behind class must be part of `CrossChildPageList.dll` (add it to the project and rebuild). For security, the module only accepts template values that are relative `.ascx` paths inside the `Template` folder.

## Project layout

| File | Purpose |
| --- | --- |
| `CrossChildPageList.csproj` / `.sln` | Visual Studio 2026 / `net472` SDK-style project and solution. |
| `View.ascx` / `View.ascx.cs` | Module entry point; loads the configured list template. |
| `Settings.ascx` / `Settings.ascx.cs` | Module settings editor. |
| `Template/default/List_Standard.ascx` / `List.ascx.cs` | Default list template and its logic. |
| `App_LocalResources/` | Localization (`.resx`) for `View` and `Settings`. |
| `images/` | `Line.gif` / `Node.gif` used by the recursive indentation prefix. |
| `CrossChildPageList.dnn` | DNN install manifest. |
| `BuildScripts/` | MSBuild targets that package and deploy the module. |
| `Resources.zip` | Generated at build time — the loose runtime files (ascx, templates, images, resx). |
| `install/` | Generated versioned install packages (`CrossChildPageList_<version>_Install.zip`). |

The `.ascx.cs` / `List.ascx.cs` code-behind is **compiled into `CrossChildPageList.dll`** by the Visual Studio project — it is no longer compiled at runtime by DNN's `BuildManager`.

## Building

Open `CrossChildPageList.sln` in Visual Studio 2026 (or build with MSBuild). Set the target DNN site in `BuildScripts/custom.xml` (`<DnnPath>`).

- **Debug** build compiles the assembly and auto-deploys the DLL + module files to the DNN site.
- **Release** build also creates the versioned install package under `install/`.

## Changelog

### v5.0.1 (2026-09-13) — compiled module

- Repackaged as a **compiled** DNN module: added a Visual Studio 2026 / .NET Framework 4.7.2 SDK-style project and MSBuild packaging/deploy scripts. The code-behind now ships in `CrossChildPageList.dll` instead of being compiled at runtime.

### v5.x (2026-09-13) — DNN 10 compatibility & fixes

- **DNN 10 support:** replaced platform APIs removed in DNN 10 — `ModuleController.GetModuleSettings`/`UpdateModuleSetting` (instance methods), `new TabController()`, `TabController.GetTab(int)`, and `GetTabsByParentId` — with their supported equivalents (`PortalModuleBase.Settings`, `ModuleController.Instance`, `TabController.Instance.GetTab(tabId, portalId)`, and static `TabController.GetTabsByParent`).
- **Fix – "Include Hidden Tab":** hidden root-level pages were being stripped before the visibility filter ran; the setting now takes effect at the root level too.
- **Restored page-permission filtering:** pages the current user cannot view are excluded again (via `TabPermissionController.CanViewPage`).
- **Security hardening:**
  - The `List Template` setting is validated before `LoadControl`, blocking path traversal to arbitrary user controls.
  - Settings save now honors `Page.IsValid`, sanitizes `Columns per row`, and whitelists `Link Target`.

### v4.0 (2018-12-10)

- Upgrade to the DNN 9.2 platform.

### Earlier

- v3.0 (2010) display child pages from menu root; remove wrap from list items.
- v2.5 (2009) added menu-styling options.
- v2.2 (2009) added "Recursive" and child-tab prefix options.
- v2.0 (2008) first free release.

## Credits & support

- Original module: [DnnModule.com](http://DnnModule.com).
- 2026 DNN 10 compatibility update: 40Fingers ([40fingers.net](https://www.40fingers.net)).

See the header comments in the `.cs` files for the detailed per-file version history.
