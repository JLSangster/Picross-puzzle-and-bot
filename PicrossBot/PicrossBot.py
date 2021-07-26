#Picross bot rules based

import pyautogui
import math
import numpy as np
import time
import sys
pyautogui.FAILSAFE = True

class puzzleModel:

    def __init__(self):
        self.parsePuzzle(self)

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
        print("Parsing complete")

    def selectCell(self,x, y):
        #work out what the xcoord and ycoord for that cell are
        (xcoord, ycoord) = (int(self.firstCell[0] + (self.cellSize * x)), int(self.firstCell[1] + (self.cellSize * y)))
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
                    self.grid[x][y] = "mark"
                    checked = True
                elif pyautogui.pixelMatchesColor(xcoord, ycoord, (0, 0, 0), tolerance = 20):
                    self.grid[x][y] = "corr"
                    checked = True
                elif pyautogui.pixelMatchesColor(xcoord, ycoord, (237, 28, 36), tolerance = 20):
                    self.grid[x][y] = "incor"
                    checked = True
                else:
                    print("oops, something went wrong with detecting the cell")
                    counter += 1
            except:
                print("oops, something went wrong.")
                counter += 1

    
    def toggleFill(self):
        #Click the toggle button
        pyautogui.click(self.togLoc)
        self.togFill = not(self.togFill)
        

    def fillCells(self, fillNum, startX, startY, rowBool, fillBool):
        #check if it fills no cells, skip
        if fillNum != 0:
            #check what mode the toggle is, and that it is what it needs to be
            if (self.togFill != fillBool):
                self.toggleFill(self)        
            for x in range(fillNum):
                if rowBool:
                    if (self.grid[startX + x][startY] == "empty"):
                        self.selectCell(self, startX + x, startY)
                else:
                    if (self.grid[startX][startY + x] == "empty"):
                        self.selectCell(self, startX, startY + x)
        

    def fillFullRow(self, coord, rowBool, fillBool):
        #check toggle is on tright mode
        #for length of whole grid
        if (self.togFill != fillBool):
            self.toggleFill(self)
        for each in range(self.size):
            #if filling a row
            if rowBool:
                if (self.grid[each, coord] == "empty"):
                    self.selectCell(self, each, coord)
            else:
                if (self.grid[coord, each] == "empty"):
                    self.selectCell(self, coord, each)

    def solve(self):
        #Check for clues that are equal to size
        print(self.clues)
        clueLoc = np.where(self.clues == self.size)
        print("full rows")
        for i in range(len(clueLoc[0])):
            rowFill = clueLoc[1][i] == 0
            self.fillFullRow(self, clueLoc[0][i], rowFill, True)

        
        #Check for "0" clues
        clueLoc = np.where(self.clues == 0)
        print("empty rows")
        for i in range(len(clueLoc[0])):
            rowFill = clueLoc[1][i] == 0
            self.fillFullRow(self, clueLoc[0][i], rowFill, False)

        #change the empty and 10 values to 0, now that all 0 clues have been handled
        self.clues = np.where(abs(self.clues) >= 10, 0, self.clues)
        
        #Clues that completely fill a row
        #first it'll have to figure out how many clues there actually are.
        print("+1 clues that fill row")
        for i in range(self.size):
            for j in range(2):
                gapCount = len(np.where(self.clues[i][j] > 0)[0]) - 1
                if ((sum(self.clues[i][j]) + gapCount) == self.size):
                    #cellplace tracks what cell in the line the bot has gotten to
                    cellPlace = 0
                    if j == 0:
                        for k in range(len(self.clues[i][j])):
                            if self.clues[i][j][k] != 0:
                                self.fillCells(self, self.clues[i][j][k], cellPlace, i, (j == 0), True)
                                cellPlace += self.clues[i][j][k]
                                if cellPlace != self.size:
                                    self.fillCells(self, 1, cellPlace, i, (j == 0), False)
                                    cellPlace += 1
                                self.clues[i][j][k] = 0
                    else:
                        for k in range(len(self.clues[i][j])):
                            if self.clues[i][j][k] != 0:
                                self.fillCells(self, self.clues[i][j][k], i, cellPlace, (j == 0), True)
                                cellPlace += self.clues[i][j][k]
                                if cellPlace != self.size:
                                    self.fillCells(self, 1, i, cellPlace, (j == 0), False)
                                    cellPlace += 1
                                self.clues[i][j][k] = 0
                                
        #A clue is more than half the size
        clueLoc = np.where(self.clues > (self.size / 2))
        print("half clues")
        print(clueLoc[0])
        for i in range(len(clueLoc[0])):
            #fillNum indicates how many either side should be missed
            fillNum = self.size - self.clues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]]
            #if clueLoc[1] == 0, its a row clue
            if clueLoc[1][i] == 0:
                self.fillCells(self, self.size - (fillNum * 2), fillNum, clueLoc[0][i], clueLoc[1][i] == 0, True)
                self.clues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]] = 0
            else:
                self.fillCells(self, self.size - (fillNum * 2), clueLoc[0][i], fillNum, clueLoc[1][i] == 0, True)
                self.clues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]] = 0

#class puzzleSolver: #Unsure if one or two classes make more sense


def main():
    puzzleModel.parsePuzzle(puzzleModel)
    puzzleModel.solve(puzzleModel)
    
if __name__ == "__main__":
    main()