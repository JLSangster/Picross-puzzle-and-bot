#Picross bot rules based

import pyautogui
import math
import numpy as np
import time
import sys
pyautogui.FAILSAFE = True

class puzzleModel:

    def __init__(self):
        self.firstCell = (0, 0)
        self.cellSize = 0
        self.togLoc = (0, 0)
        self.togFill = True
        self.size = 0
        self.clues = np.empty([0, 2, 0], int)
        self.grid = np.full([0, 0], "empty")
        self.workingClues = self.clues
        self.parsePuzzle()

    def parsePuzzle(self):
        print("Parsing")
        #click on window to ensure puzzle is in focus
        pyautogui.click(10,10)
        #Using a try catch to close the program if there is no puzzle
        try:
            #identify the puzzle via image recognition of the empty cells.
            cellLoc = list(pyautogui.locateAllOnScreen('cell.png', confidence = 0.95))
            #store the coord of first cell. this is cell (0,0).
            self.firstCell = pyautogui.center(cellLoc[0])
            pyautogui.moveTo(self.firstCell)
            self.cellSize = abs(self.firstCell[0] - pyautogui.center(cellLoc[1])[0])
        except IndexError:
            sys.exit("Error - no picross puzzle detected. Ensure that the picross puzzle is visible on the current screen.")
        except:
            sys.Exit("Unknown Error occurred")

        #locate the toggle
        self.togLoc = pyautogui.locateCenterOnScreen('toggle.png')
        self.togFill = True

        #Maximum error is one either way
        if math.isqrt(len(cellLoc)) in range(4, 6):
            self.size = 5
        elif math.isqrt(len(cellLoc)) in range(9,11):
            self.size = 10
        else:
            print("err")

        maxClue = int(math.ceil(self.size/2))
        self.clues = np.empty([self.size, 2, maxClue], int)
        for r in range(self.size):
            #rows first
            for clue in range(maxClue, 0 , -1):
                #move to cell just for user feedback
                pyautogui.moveTo(((self.firstCell[0] - (clue * self.cellSize)), self.firstCell[1] + (r * self.cellSize)))
                #calculate the region to search in
                reg = (pyautogui.position()[0] - int(self.cellSize / 2), pyautogui.position()[1] - int(self.cellSize / 2), self.cellSize, self.cellSize)
                
                #check for each of the clues
                for i in range(self.size + 1):
                    if pyautogui.locateOnScreen((str(i) + ".png"),region = reg, confidence = 0.85) != None:
                        self.clues[r,0,abs(clue-maxClue)] = i
                        break

                #same again for columns
                pyautogui.moveTo(((self.firstCell[0] + (r * self.cellSize), self.firstCell[1] - (clue * self.cellSize))))
                reg = (pyautogui.position()[0] - int(self.cellSize / 2), pyautogui.position()[1] - int(self.cellSize / 2), self.cellSize, self.cellSize)
                
                #check for each of the clues
                for i in range(self.size + 1):
                    if pyautogui.locateOnScreen((str(i) + ".png"),region = reg, confidence = 0.85) != None:
                        self.clues[r,1,abs(clue-maxClue)] = i
                        break

        #create the mats
        #A square mat of the grid
        #four possible values: empty, marked, incorrect, correct
        self.grid = np.full((self.size, self.size), "empty")
        #workingClues holds clues that are still relevant, e.g, clues that have not been fulfilled,
        #avoids solver filling cells that have allready been filled by a previous rule, and to go back and check what those clues were
        self.workingClues = self.clues
        print("Parsing complete")

    #def solve(self):
        #print(self.workingClues)
        #for i in range(self.size):
        #    print(i)
        #    clueCounter = 0
        #    while clueCounter <= len(self.clues[0][0]):
        #        if self.workingClues[i][0][clueCounter] != 0:
        #            for l in range(self.size):
        #                filledCount = 0
                        #find the first filled cell.
        #                if self.grid[i][l] == "corr":
        #                    filledCount += 1
        #                else:
        #                    if filledCount != 0:
        #                        if self.workingClues[i][0][clueCounter] == filledCount:
        #                            self.workingClues[i][0][clueCounter] = 0
        #                        clueCounter += 1
                        #count how many cells after that are also filled.
        #        else:
        #            clueCounter += 1

        #    while clueCounter <= len(self.clues[0][0]):
        #        if self.workingClues[i][1][clueCounter] != 0:
        #            for l in range(self.size):
        #                filledCount = 0
                        #find the first filled cell.
        #                if self.grid[i][l] == "corr":
        #                    filledCount += 1
        #                else:
        #                    if filledCount != 0:
        #                        if self.workingClues[i][1][clueCounter] == filledCount:
        #                            self.workingClues[i][1][clueCounter] = 0
        #                        clueCounter += 1
        #        else:
        #            clueCounter += 1
        #print(self.workingClues)

        

class puzzleSolver:
    def selectCell(self, puzzleModel, x, y):
        #work out what the xcoord and ycoord for that cell are
        (xcoord, ycoord) = (int(puzzleModel.firstCell[0] + (puzzleModel.cellSize * x)), int(puzzleModel.firstCell[1] + (puzzleModel.cellSize * y)))
        pyautogui.click(xcoord, ycoord)

        #if self.grid[x][y] == "empty":
        #read what the cell changes to
        #given that the mouse should be in the center of the cell, we can use the colour of that pixel to decide
        #gray is marked, red is incorrect, black is correct
        counter = 0
        checked = False
        while not(checked):
            #add timer to prevent infinite loop
            if counter > 10:
                #change to error message
                print("Stuck in loop. please close and retry.")
                break
            #Due to an issue in pyautogui, repeat the pixel check until this identifies the new cell status
            try:
                if pyautogui.pixelMatchesColor(xcoord,ycoord, (127, 127, 127), tolerance = 20):
                    puzzleModel.grid[x][y] = "mark"
                    checked = True
                elif pyautogui.pixelMatchesColor(xcoord, ycoord, (0, 0, 0), tolerance = 20):
                    puzzleModel.grid[x][y] = "corr"
                    checked = True
                elif pyautogui.pixelMatchesColor(xcoord, ycoord, (237, 28, 36), tolerance = 20):
                    puzzleModel.grid[x][y] = "incor"
                    checked = True
                else:
                    print("oops, something went wrong with detecting the cell")
                    counter += 1
            except:
                print("oops, something went wrong.")
                counter += 1

    
    def toggleFill(self, puzzleModel):
        #Click the toggle button
        pyautogui.click(puzzleModel.togLoc)
        puzzleModel.togFill = not(puzzleModel.togFill)
        

    def fillCells(self, puzzleModel, fillNum, startX, startY, rowBool, fillBool):
        #check if it fills no cells, skip
        if fillNum != 0:
            #check what mode the toggle is, and that it is what it needs to be
            if (puzzleModel.togFill != fillBool):
                self.toggleFill(self, puzzleModel)        
            for x in range(fillNum):
                if rowBool:
                    if (puzzleModel.grid[startX + x][startY] == "empty"):
                        self.selectCell(self, puzzleModel, startX + x, startY)
                else:
                    if (puzzleModel.grid[startX][startY + x] == "empty"):
                        self.selectCell(self, puzzleModel, startX, startY + x)
        

    def fillFullRow(self, puzzleModel, coord, rowBool, fillBool):
        #check toggle is on tright mode
        #for length of whole grid
        if (puzzleModel.togFill != fillBool):
            self.toggleFill(self, puzzleModel)
        for each in range(puzzleModel.size):
            #if filling a row
            if rowBool:
                if (puzzleModel.grid[each, coord] == "empty"):
                    self.selectCell(self, puzzleModel, each, coord)
            else:
                if (puzzleModel.grid[coord, each] == "empty"):
                    self.selectCell(self, puzzleModel, coord, each)

    def phaseOne(self, puzzleModel):
        print(puzzleModel.clues)
        
        #Check for "0" clues
        clueLoc = np.where(puzzleModel.workingClues == 0)
        print("empty rows")
        for i in range(len(clueLoc[0])):
            rowFill = clueLoc[1][i] == 0
            self.fillFullRow(self, puzzleModel, clueLoc[0][i], rowFill, False)
            puzzleModel.workingClues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]] = 0
            
        #Check for clues that are equal to size
        clueLoc = np.where(puzzleModel.workingClues == puzzleModel.size)
        print("full rows")
        for i in range(len(clueLoc[0])):
            rowFill = clueLoc[1][i] == 0
            self.fillFullRow(self, puzzleModel, clueLoc[0][i], rowFill, True)
            puzzleModel.workingClues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]] = 0

        #change the empty and 10 values to 0, now that all 0 clues have been handled
        puzzleModel.workingClues = np.where(abs(puzzleModel.workingClues) >= 10, 0, puzzleModel.workingClues)

    def phaseTwo(self, puzzleModel):
        #check for clues that have been filled.
        print("start")
        #for each column
        for x in range(puzzleModel.size):
            corrCount = 0
            #count the number of correct cells in the column
            for y in range(puzzleModel.size):
                if puzzleModel.grid[x][y] == "corr":
                    corrCount += 1
            #if the number of correct clues = the sum of the clues
            if corrCount == np.sum(np.where(abs(puzzleModel.clues[x][1]) <= puzzleModel.size, puzzleModel.clues[x][1], 0)):
                for y in range(puzzleModel.size):
                    if puzzleModel.grid[x][y] == "empty":
                        self.fillCells(self, puzzleModel, 1, x, y, False, False)
                np.where(puzzleModel.workingClues[x][1] != 0, 0, 0)

        for y in range(puzzleModel.size):
            corrCount = 0
            #count the number of correct cells in the column
            for x in range(puzzleModel.size):
                if puzzleModel.grid[x][y] == "corr":
                    corrCount += 1
            #if the number of correct clues = the sum of the clues
            if corrCount == np.sum(np.where(abs(puzzleModel.clues[y][0]) <= puzzleModel.size, puzzleModel.clues[y][1], 0)):
                for y in range(puzzleModel.size):
                    if puzzleModel.grid[x][y] == "empty":
                        self.fillCells(self, puzzleModel, 1, x, y, False, False)
                np.where(puzzleModel.workingClues[y][0] != 0, 0, 0)

    def phaseThree(self, puzzleModel):
        #Clues that completely fill a row
        #first it'll have to figure out how many clues there actually are.
        print("+1 clues that fill row")
        for i in range(puzzleModel.size):
            for j in range(2):
                gapCount = len(np.where(puzzleModel.workingClues[i][j] > 0)[0]) - 1
                if ((sum(puzzleModel.workingClues[i][j]) + gapCount) == puzzleModel.size):
                    #cellplace tracks what cell in the line the bot has gotten to
                    cellPlace = 0
                    if j == 0:
                        for k in range(len(puzzleModel.workingClues[i][j])):
                            if puzzleModel.workingClues[i][j][k] != 0:
                                self.fillCells(self, puzzleModel, puzzleModel.workingClues[i][j][k], cellPlace, i, (j == 0), True)
                                cellPlace += puzzleModel.workingClues[i][j][k]
                                if cellPlace != puzzleModel.size:
                                    self.fillCells(self, puzzleModel, 1, cellPlace, i, (j == 0), False)
                                    cellPlace += 1
                                puzzleModel.workingClues[i][j][k] = 0
                    else:
                        for k in range(len(puzzleModel.workingClues[i][j])):
                            if puzzleModel.workingClues[i][j][k] != 0:
                                self.fillCells(self, puzzleModel, puzzleModel.workingClues[i][j][k], i, cellPlace, (j == 0), True)
                                cellPlace += puzzleModel.workingClues[i][j][k]
                                if cellPlace != puzzleModel.size:
                                    self.fillCells(self, puzzleModel, 1, i, cellPlace, (j == 0), False)
                                    cellPlace += 1
                                puzzleModel.workingClues[i][j][k] = 0
                                                        
        #A clue is more than half the size
        clueLoc = np.where(puzzleModel.workingClues > (puzzleModel.size / 2))
        print("half clues")
        print(clueLoc[0])
        for i in range(len(clueLoc[0])):
            #fillNum indicates how many either side should be missed
            fillNum = puzzleModel.size - puzzleModel.workingClues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]]
            #if clueLoc[1] == 0, its a row clue
            if clueLoc[1][i] == 0:
                self.fillCells(self, puzzleModel, puzzleModel.size - (fillNum * 2), fillNum, clueLoc[0][i], clueLoc[1][i] == 0, True)
                #clue should not be set to 0 as the full clue has not been fulfilled
            else:
                self.fillCells(self, puzzleModel, puzzleModel.size - (fillNum * 2), clueLoc[0][i], fillNum, clueLoc[1][i] == 0, True)
                #clue should not be set to 0 as the full clue has not been fulfilled
                
        #check for cells on the edges filled
        for x in range(puzzleModel.size):
            if puzzleModel.grid[x][0] == "corr":
                #read the first clue for that 
                print(x)
                clue = puzzleModel.workingClues[x][1][0]
                self.fillCells(self, puzzleModel, clue, x, 0, False, True)
                self.fillCells(self, puzzleModel, 1, x, clue, False, False)
            
            if puzzleModel.grid[x][-1] == "corr":
                print(x)
                clue = puzzleModel.workingClues[x][1][-1]
                self.fillCells(self, puzzleModel, clue, x, puzzleModel.size - clue, False, True)
                self.fillCells(self, puzzleModel, 1, x, puzzleModel.size - clue - 1, False, False)

        for y in range(puzzleModel.size):
            if puzzleModel.grid[0][y] == "corr":
                print(y)
                #read the first clue for that 
                clue = puzzleModel.workingClues[y][0][0]
                self.fillCells(self, puzzleModel, clue, 0, y, True, True)
                self.fillCells(self, puzzleModel, 1, clue, y, True, False)
            
            if puzzleModel.grid[-1][y] == "corr":
                print(y)
                clue = puzzleModel.workingClues[y][0][-1]
                self.fillCells(self, puzzleModel, clue, puzzleModel.size - clue, y, True, True)
                self.fillCells(self, puzzleModel, 1, puzzleModel.size - clue - 1, y, True, False)

    def solve(self, puzzleModel):
        self.phaseOne(self, puzzleModel)
        self.phaseTwo(self, puzzleModel)
        self.phaseThree(self, puzzleModel)

def main():
    model = puzzleModel()
    puzzleSolver.solve(puzzleSolver, model)
    print("end")
    
if __name__ == "__main__":
    main()