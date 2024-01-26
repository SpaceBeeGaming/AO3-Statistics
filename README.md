# AO3-Statistics

A small-ish AO3 user statistic crawler.

Currently, the program collects the data into `.csv` files by work. If you'd like the data in another format, you can create a feature request, and I'll see what I can do.

You **must** have an account on AO3, and you can only collect your own statistics (unless you know your friend's password :wink:).

I've designed the program to be easy to automate, for example, using the Windows Task Scheduler. Alternatively, you can run it interactively if you do not want to save your AO3 password on your device.

[![CodeQL](https://github.com/SpaceBeeGaming/AO3-Statistics/actions/workflows/github-code-scanning/codeql/badge.svg?branch=master)](https://github.com/SpaceBeeGaming/AO3-Statistics/actions/workflows/github-code-scanning/codeql)

## Installation

The program doesn't need installation in the traditional sense. The only requirement is that the program executable and the `appsettings.json` file are in the same folder.

Just download the zip (Windows x64) file from the [Releases page](https://github.com/SpaceBeeGaming/AO3-Statistics/releases/latest) and extract it to a preferably empty folder.

You can also manually compile the project if you want it for other operating systems.

## Configuration

Most program configuration is done through the `appsettings.json` file, which can be edited using `notepad` or a similar raw text editor.

If you desire, this configuration can be entered as command-line arguments, which will override values set in the config file.

### `appsettings.json` file

The relevant portion of the configuration file is as follows:

```json
 "UserOptions": {
    "Username": "",
    "Password": "", // Do NOT use for plain text password. 
    "PasswordIsProtected": false // Do not touch unless instructed.
  },
  "OutputOptions": {
    "FolderPath": "./Data",
    "OutputFormat": "MultiCSV", // Valid values: MultiCSV,
    "OutputCulture": "en-us"
  },
```

Property | Description
:---: | ---
`Username` | Your AO3 username and **is** Case sensitive.
`Password` | Used for the automated mode. Do **NOT** store the plain text password. Instead, use the encrypted variant provided when running in manual mode (see below).
`PasswordIsProtected` | Used as a flag to tell the program how to handle the provided password. This also hides the instructions for enabling automated mode if you prefer to use the program exclusively in manual mode.
`FolderPath` | The folder where the program output will be stored. You can change it to your liking. This directory must exist, and you must have permission to access it. The directory will **not** be created if it is missing.
`OutputFormat` | The format in which the data is saved. Please take a look below for valid values.
`OutputCulture`| The culture to use for output. This will affect date formatting, number formatting, and other `OutputFormat` specific details. Generally, you will want to set this according to the language you use in the program you read the data in. For example, you'll want this to match the display language of MS Excel if you use it to interpret the data; otherwise, Excel may not read the CSV correctly, or you will have to manually specify how the CSV is formatted.

#### Valid values for `OutputFormat`

OutputFormat | Description
:---: | ---
`MultiCSV` | The data is written in individual CSV files, one for each work and one for user statistics.

## Running the program

You can use the program in two modes:

- _Manual_ by running it though your terminal with `AO3Statistics.exe --Password` and following the directions. This requires you to input your password every time.  
  I have provided a cmd script that runs this exact command and keeps the window open.
- _Automated_ by following the instructions provided when running interactively.
  - After that, you can use whatever method to periodically execute it.  
    For example, the Windows Task Scheduler might be the most convenient.

## A basic overview of program operation

1. Login to AO3 using your credentials.
2. Visit your statistics page, i.e., `archiveofourown.org/users/<USERNAME>/stats`.
3. Extract the statistics information from the HTML code.
4. Logout
5. Save data to files.
6. Exit

## Feature requests and bug reports

Please create them if you have any issues or changes you'd like made. Pull requests are also welcome, but **do** make an issue first.

Also, make an issue if you have problems setting up the program.
