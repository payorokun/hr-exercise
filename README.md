# Code Challenge

## Overview

The goal is to show off what you can do. Be creative! There is a specific problem to solve, but you are welcome to play around with it and add things that we didn't ask for. 

Your code will be shared among the team here, and we'll review it together during the in-person interview.

## The Problem

You are being asked to implement a Web API that lets you manage an archive that stores notable quotations. At minimum, you will need to be able to perform basic CRUD (Create, Read, Update, Delete) operations on Quotes. 

However the main problem is that you've also received a requirement that you need to also provide an additional route that will find the number of pairs of Quotes in your archive that will fit into a text field of a given length. More formally,

Input: A numeric value representing a length of characters in a string
Expected result: The total number of unique pairs of Quotes in your archive that satisfy the following condition:
> Two quotes qualify if the sum of the length of the text of the two quotes is less than or equal to the provided input.


## Additional Considerations

### Scaling

The provided archive, ShortDb.json, only has a few entries in it. Naturally, any algorithm that solves the problem will work on it. However, we've also provided a second file, LargeDb.json, that contains many more entries already present in the archive. Inefficient code won't handle this well. See if you can still handle this larger file.

If you end up generating any metadata for the provided files, please also provide your code for how this was generated even if that code is separate from the Web API.

### Framework level stuff (e.g. Logging, Exception Handling, Configuration)

 There's no need to make this production ready, but maybe be prepared to talk about these considerations in person.

### Testing

It would be helpful, but not required, to provide some form of unit test(s) or integration test(s) to prove that your code works.

### New Features

If you want to show off your skills in a different area, feel free to create your own features and implement them! 

Some possible ways to add features:

Add a web interface to the API.  The core challenge here is on the backend side, but if you want to show off your web development abilities this is your opportunity.

Create a Person object, and replace the Author field of the Quote object with a reference to a Person. Then provide CRUD operations to manage People.

## Sample Inputs

With the provided ShortDb.json, here's a few sample inputs and outputs for the new route that should be created:

* Input: 19, Output: 0
* Input: 22, Output: 1
* Input: 32, Output: 6
* Input: 40, Output: 7
* Input: 200, Output: 21
