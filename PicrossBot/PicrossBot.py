#Picross bot rules based

import pyautogui
import math
import numpy as np

class puzzleModel:

    def __init__(self):
        self.parsePuzzle(self)

    def parsePuzzle(self):
        print("called")
        #find the size of the grid
        #count the cells it finds
        cellLoc = list(pyautogui.locateAllOnScreen('cell.png', confidence = 0.95))
        #store the coord of first cell. this is 0,0.
        self.firstCell = pyautogui.center(cellLoc[0])
        self.cellSize = abs(self.firstCell[0] - pyautogui.center(cellLoc[1])[0])

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
                print(clue)
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
                pyautogui.moveTo(((self.firstCell[0] + (r * self.cellSize), self.firstCell[1] - (clue * self.cellSize))))
                reg = (pyautogui.position()[0] - int(self.cellSize / 2), pyautogui.position()[1] - int(self.cellSize / 2), self.cellSize, self.cellSize)
                
                #check for each of the clues - there is probably a better way to do this.
                for i in range(10):
                    if pyautogui.locateOnScreen((str(i) + ".png"),region = reg, confidence = 0.9) != None:
                        self.clues[r,1,abs(clue-maxClue)] = i
                        break



        #create the mats
        #A square mat of the grid
        #four possible values, marked, marked, incorrect, correct
        #empty, marked, incorrect, correct
        self.grid = np.full((self.size, self.size), "empty")
        #a matrix of the clues - similar to the one in the game
        


    def selectCell(self,row, col):
        #work out what the xcoord and ycoord for that cell are
        pyautogui.click(xcoord, ycoord)
        #read what the cell changes to
        #given that the mouse should be in the center of the cell, we can use the colour of that pixel to decide
        #gray is marked, red is incorrect, black is correct
        if pyautogui.pixelMatchesColor(xcoord,ycoord, (127, 127, 127), tolerance = 10):
            self.grid[row][col] = "marked"
        elif pyautogui.pixelMatchesColor(xcoord, ycoord, (0, 0, 0), tolerance = 10):
            self.grid[row][col] = "correct"
        elif pyautogui.pixelMatchesColor(xcoord, ycoord, (237, 28, 36), tolerance = 10):
            self.grid[row][col] = "incorrect"
        else:
            print(err)
    
    def toggleFill(self):
        #work out where on the screen the toggle is
        pyautogui.click(xcoord, ycoord)

    def fillCells(self,fillNum, startX, startY, rowBool, fillBool):
        #check what mode the toggle is, and that it is what it needs to be
        for x in range(fillNum):
            if rowBool:
                selectCell(startX + x, startY)
            else:
                selectCell(startX, startY + x)

    #def markFullRow(row, col, rowBool):
        #check toggle is on tright mode
        #for length of whole grid
        #if rowBool
        #selectCell(row, x)
        #else
        #selectCell(x, col)

def main():
    puzzleModel.parsePuzzle(puzzleModel)
    #puzzleModel.selectCell(puzzleModel,1,1)

if __name__ == "__main__":
    main()