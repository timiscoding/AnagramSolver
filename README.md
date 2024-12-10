# Anagram Solver

<img width="1803" alt="SCR-20241211-jeip" src="https://github.com/user-attachments/assets/f93f792e-74be-47d8-ab64-45b972a3bc9c">

Originally created this as a tool to 'help' me play the game Words on Stream. It's a dotnet CLI app for solving anagrams given a 4-9 letter pattern with wildcard support, and shows anagrams ordered by most frequent used words and in dictionary order.

## Usage

1. Configure and download a word list text file http://app.aspell.net/create For a comprehensive word list, I used the following settings: size 95 (insane) on US, GB dictionaries, common variants and stripped diacritics, no special lists
2. Copy the word list file into `bin/Debug/net8.0/dictionaries/word-list.txt`
3. Generate single word (1 gram) word frequency csv from Google corpora https://github.com/orgtre/google-books-ngram-frequency
4. Copy the frequency list into `bin/Debug/net8.0/dictionaries/1grams_english2.csv`
5. Export the WOS word list into a text file from https://docs.google.com/spreadsheets/d/1MD-1W-JFjGZN4jgk9Pr1lx2jalPlAkkgqkgJRx9h7Y4/edit?gid=2031547997#gid=2031547997
6. Copy it to `bin/Debug/net8.0/dictionaries/wos_word_list2.txt`

Run the app `dotnet AnagramSolver.dll`
