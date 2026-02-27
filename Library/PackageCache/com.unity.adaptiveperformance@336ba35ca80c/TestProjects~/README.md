# Adaptive Performance Tech Demo

The tech demo project has one project which houses demos without URP and for demos using URP, use the correct project setting to create update the samples.

## About Samples

Adaptive Performance 2.0 ships with several samples which are currently commited in this repo.

For every release the content of the Assets folder is copied and replace in the Samples~ folder of com.unity.adaptiveperformance so make sure to fill out all the data. needed.

If you add a new sample, make a new folder and copy the .sample.json and adapt. Once the content is synced to Samples~ the content of this json file needs to be copied over to the package manifest.json of the com.unity.adaptiveperformance package. Ensure you have the right dependencies linked in the .sample.json

Following folders are not moved over to Samples~
- SceneMenu-donotship
- Adaptive Performance

For each demo you need to add a description to the samples-guide.md of com.unity.adaptiveperformance Documentation~ Ensure you have the correct URP/non URP buides.

## About URP
How to work with URP assets as the default project is Unity built-in Render Pipeline, as most demos should work in any Unity project.

Adaptive Performance requires Universal Render Pipeline versions `7.5`, `8.2`, `10.0` and later. Install it via the [Unity Package Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html).

To use the Universal Render Pipeline Settings we ship with the samples you have to follow following steps:

- Go to **Project Settings &gt; Graphics &gt; Scraiptable Render Pipeline Settings** and add the Universal Render Pipeline Asset from `Sample Environment/URP Settings/APSamplesHighQuality.asset`.
- Go to **Project Settings &gt; Quality &gt; Rendering** and add the Universal Render Pipeline Asset from `Sample Environment/URP Settings/APSamplesHighQuality.asset`.
- Convert the Assets from Unit built-in pipeline to Universal Render Pipeline. Go to **Edit &gt; Render Pipeline &gt; Universal Render Pipeline &gt; Upgrade Project Materials to UniversalRP Materials** from Unity's main menu.

Done - but ensure not to commit any converted files.

When committing, do not commit any converted files. ensure to only commit new files. If you add new content, do use standard shaders and Built-in Materials and nothing specific to URP so they can be used in other samples without URP as well. Ensure they can be converted to URP without issues, otherwise refrain from using them.

Currently requires Unity 2020.1 (URP 8.2) or 2020.2 (URP 10) - we still wait to Unity 2019.LTS with (URP 7.5).

## Technical details

### Synlinks
The Samples projects use symlinks for the Assets folder to the Sample~ folder.
On mac and linux it should work by default, on Windows you will need to start the console or your SCM programm in Administrator mode so it can create the symlinks.

If you did not clone the repo with symlinks enabled in admin mode:

git clone -c core.symlinks=true <URL>

you will have to enable them in the repo. It might not work local only, in that case you might also change it in your global git config.

git config --global core.symlinks true
git config --local core.symlinks true

After a branch change it will generate symlinks. (if it's not a symlink, it is a text file with the path saved)


### Requirements

This version of the Adaptive Performance Tech Demo is compatible with the following versions of the Unity Editor:

* 2019 LTS and later (recommended)

This version of the Adaptive Performance Tech Demo is compatible with the following versions of Adaptive Performance:

* 2.0.0
