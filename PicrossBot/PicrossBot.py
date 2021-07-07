#Picross bot rules based

import pyautogui
import math
import numpy as np
import time
pyautogui.FAILSAFE = True

class puzzleModel:

    def __init__(self):
        self.parsePuzzle(self)

    def parsePuzzle(self):
        print("Parsing")
        #click on window to ensure puzzle is in focus
        pyautogui.click(10,10)
        #find the size of the grid
        #count the cells it finds
        cellLoc = list(pyautogui.locateAllOnScreen('cell.png', confidence = 0.95))
        #store the coord of first cell. this is 0,0.
        self.firstCell = pyautogui.center(cellLoc[0])
        pyautogui.moveTo(self.firstCell)
        self.cellSize = abs(self.firstCell[0] - pyautogui.center(cellLoc[1])[0])

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
                
                #check for each of the clues - there is probably a better way to do this.
                for i in range(10):
                    if pyautogui.locateOnScreen((str(i) + ".png"),region = reg, confidence = 0.85) != None:
                        self.clues[r,0,abs(clue-maxClue)] = i
                        break

                #same again for columns
                reg = (pyautogui.position()[0] - int(self.cellSize / 2), pyautogui.position()[1] - int(self.cellSize / 2), self.cellSize, self.cellSize)
                
                #check for each of the clues - there is probably a better way to do this.
                for i in range(self.size):
                    if pyautogui.locateOnScreen((str(i) + ".png"),region = reg, confidence = 0.9) != None:
                        self.clues[r,1,abs(clue-maxClue)] = i
                        break

        #create the mats
        #A square mat of the grid
        #four possible values, marked, marked, incorrect, correct
        #empty, marked, incorrect, correct
        self.grid = np.full((self.size, self.size), "empty")
        print("Parsing complete")

    def selectCell(self,x, y):
        #work out what the xcoord and ycoord for that cell are
        (xcoord, ycoord) = (int(self.firstCell[0] + (self.cellSize * x)), int(self.firstCell[1] + (self.cellSize * y)))
        pyautogui.click(xcoord, ycoord)

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
        

    def fillCells(self,fillNum, startX, startY, rowBool, fillBool):
        #check what mode the toggle is, and that it is what it needs to be
        if (self.togFill != fillBool):
            self.toggleFill(self)        
        for x in range(fillNum):
            if rowBool:
                self.selectCell(self, startX + x, startY)
            else:
                self.selectCell(self, startX, startY + x)
        

    def fillFullRow(self, x, y, rowBool, fillBool):
        #check toggle is on tright mode
        #for length of whole grid
        if (self.togFill != fillBool):
            self.toggleFill(self)
        for each in range(self.size):
            #if filling a row
            if rowBool:
                if (self.grid[each, y] == "empty"):
                    self.selectCell(self, each, y)
            else:
                if (self.grid[x, each] == "empty"):
                    self.selectCell(self, x, each)

def main():
    puzzleModel.parsePuzzle(puzzleModel)
    puzzleModel.fillFullRow(puzzleModel, 2, 3, False, True)
    puzzleModel.fillFullRow(puzzleModel, 4, 0, True, False)
    
if __name__ == "__main__":
    main()