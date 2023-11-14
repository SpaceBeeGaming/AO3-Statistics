# AO3-Statistics
A small AO3 user statistic crawler.

I created this program to keep track of reader engagement and the word counts of my writing on AO3.

Currently, the program will collect the data into `.csv` files by work. If you'd like to get the data in another format, you can create a feature request, and I'll see what I can do.

You **must** have an account on AO3, and you can only collect your statistics (unless you know your friend's password ;)).

I've designed the program to be easily automatable, for example, by using the Windows Task Scheduler. Or you can run it interactively if you do not want to save your AO3 password on your device.

**Program operation:**
1. Login to AO3 using your credentials.
2. Visit your statistics page, i.e., `archiveofourown.org/users/<USERNAME>/stats`.
3. Extract the statistics information from the HTML code.
4. Logout
5. Save data to files.
6. Exit

## Feature requests and bug reports
Please do create them if you have any issues or changes you'd like made. Pull requests are also welcome but **do** make an issue first.
