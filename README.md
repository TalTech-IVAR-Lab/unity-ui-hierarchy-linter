# Unity Package Template Repository

This is a template for creating packages for [Unity Package Manager (UPM)](https://docs.unity3d.com/Manual/Packages.html).
It provides the basic package structure as well as ready GitLab CI configuration for deploying this package to an NPM registry.

## How to use

You can use this repo to bootstrap the creation of a new UPM package:

1. Clone/download this repository.
2. Delete `.git` folder - you will create a new repo for the package anyway.
3. Update package metadata in _package.json_.
4. Update name and year in _LICENSE.md_.
5. Update this _README.md_ with the description of the package.
6. Initialize a new git repository inside this folder.
7. Open Unity and add this folder as a local package through UPM.
8. Start developing!

## Attributions

This repository is inspired by [unity-package-template](https://github.com/3d-group/unity-package-template) repository created by [3D Group](https://github.com/3d-group).
