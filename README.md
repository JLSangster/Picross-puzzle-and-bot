# Picross-puzzle-and-bot, by JLSangster.
Unity based bare-bones picross puzzle game with python based bot to solve using rules-based AI

In it's current state, this project is a small picross puzzle game. 

RULES
In this game, each new puzzle is randomly generated as either a 5x5 or 10x10 puzzle.
As with traditional picross or nonagram puzzles, the goal is to fill every "correct" cell to make a pattern, using the clues on each row and column.

Each clue works as follows;
  Row clues should be read left to right, and column clues should be read from top to bottom.
  
  Each clue is comprised of one or more numbers. These numbers indicate the number of adjacent cells in that row are filled, with a gap of unfilled cells
  between each number. There may be a gap of any possible size before the first filled cell, or after the last in a given row.
  
  For example:
  If a clue for a 5 cell row reads:         
      3 1
  This indicates that within this row there is a row of three adjacent filled cells, followed by a gap of unknown size, and then followed by single filled cell.
  
  ■ ■ ■ □ ■
  
  Would be the correct solution for that row.
  
Optional 'x' markers may be used to indicate that a cell should nto be filled, but are not required to complete the puzzle.
  
When an incorrect cell is filled, a life is lost. In 5x5 puzzles, there are two lives, in 10x10 puzzles, there are three. Should a player lose all of their lives,
the puzzle is failed.


CONTROLS
A toggle on the lower right of the puzzle can be left clicked to switch between filling and marking cells.
Left clicking on a cell will either mark or fill it.
If a cell is already marked, it must be unmarked before filling.
If a cell has been incorrectly filled, it will be marked with a red 'x' that cannot be interacted with.

GAME DATA
As this is intended as a platform to be used for creating and testing a puzzle solving AI, the option to locally save some data from the play session is given.
This data is as follows: 
  The number of puzzles successfully completed.
  The number of puzzles lost.
  The average time to successfully complete a puzzle, in seconds.
  The average time before a puzzle was lost, in seconds.
This data is saved into a csv file named "Picross_results.csv" in the program folder only.
  
INSTALLING
The current build is available in the PicrossBuild folder. Downloading this into programs or Desktop and launching the Picross.exe inside will allow the game to run with full file permissions, allowing the results csv to be saved. In other locations the game will run without this feature.
