# SimpleBmp : le format Bitmap minimaliste

## Introduction

Bmp2 est un format d'image minimaliste et simpliste.
Il s'agit d'un exercice et n'a pas vocation à être utilisé en production.

## Fonctionnalités

Ce projet permet :

- Créer, manipuler un objet Bmp2
- Sauvegarde et lecture d'un fichier Bmp2
- Lecture de fichier texte pour importer une grille de pixel 
- Conversion en format Bitmap
- Affichage via un visualiseur minimaliste


## Spécifications du format Bmp2

Le format d'image Bmp2 contient :

- Nombre magique (Bmp2) (4 octets)
- Largeur de l'image (2 octets)
- Hauteur de l'image (2 octets)
- Nombre d'octets par pixel (2 octets)
- Nombre d'octets par couleur (2 octets)
- Nombre de couleur dans la palette (4 octets)
- Définition de la palette
- Grille de pixel

La palette est une suite de couleur, chaque couleur étant définie sur 4 octets, un octet par canal dans l'ordre : R G B A

La grille de pixel est une suite d'octet, la place de chaque octet définissant sa place dans l'image et sa valeur le numéro de couleur de la palette


## Détails techniques du projet
- .Net 6.0
- OS cible : windows
- Visual Studio 2022

## Comment utiliser
- Cloner ou télécharger ce projet
- Ouvrir et générer SimpleBmp
- Dans le projet cible, ajouter en référence le fichier SimpleBmp.dll
- Code simple de test :

	using SimpleBmp

	Bmp2 b = new Bmp2(20, 20);

	for (int i = 0; i < b.Width; i++)
	{
	    for (int j = 0; j < b.Height; j++)
	    {
	        b.SetPixel(i, j, (byte)(i % 7));
	    }
	}

	Bmp2Viewer viewer = new Bmp2Viewer(b.Width, b.Height, 5);
	viewer.Show(b);

