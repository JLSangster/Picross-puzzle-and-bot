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
            sys.exit("Unknown Error occurred")

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
                    if pyautogui.locateOnScreen((str(i) + ".png"),region = reg, confidence = 0.87) != None:
                        self.clues[r,0,abs(clue-maxClue)] = i
                        break

                #same again for columns
                pyautogui.moveTo(((self.firstCell[0] + (r * self.cellSize), self.firstCell[1] - (clue * self.cellSize))))
                reg = (pyautogui.position()[0] - int(self.cellSize / 2), pyautogui.position()[1] - int(self.cellSize / 2), self.cellSize, self.cellSize)
                
                #check for each of the clues
                for i in range(self.size + 1):
                    if pyautogui.locateOnScreen((str(i) + ".png"),region = reg, confidence = 0.87) != None:
                        self.clues[r,1,abs(clue-maxClue)] = i
                        break

        #create the mats
        #A square mat of the grid
        #four possible values: empty, marked, incorrect, correct
        self.grid = np.full((self.size, self.size), "empty")
        #workingClues holds clues that are still relevant, e.g, clues that have not been fulfilled,
        #avoids solver filling cells that have allready been filled by a previous rule, and to go back and check what those clues were
        self.workingClues = self.clues
        print(self.clues)
        print("Parsing complete")  
        print(self.grid.shape)      

class puzzleSolver:
    def selectCell(self, puzzleModel, x, y):
        #work out what the xcoord and ycoord for that cell are
        (xcoord, ycoord) = (int(puzzleModel.firstCell[0] + (puzzleModel.cellSize * x)), int(puzzleModel.firstCell[1] + (puzzleModel.cellSize * y)))
        pyautogui.click(xcoord, ycoord)

        #read what the cell changes to
        #given that the mouse should be in the center of the cell, we can use the colour of that pixel to decide
        #gray is marked, red is incorrect, black is correct - in theory this should be faster than the image rec used earlier
        counter = 0
        checked = False
        while not(checked):
            #add timer to prevent infinite loop
            if counter > 10:
                #change to error message
                print("Stuck in loop. please close and retry.")
                break
            try:
                reg = (pyautogui.position()[0] - int(puzzleModel.cellSize / 2), pyautogui.position()[1] - int(puzzleModel.cellSize / 2), puzzleModel.cellSize, puzzleModel.cellSize)
                checked = True
                if pyautogui.locateOnScreen("incor.PNG", region = reg, confidence = 0.85):
                    puzzleModel.grid[y][x] = "incor"
                    checked = True
                elif pyautogui.locateOnScreen("corr.PNG", region = reg, confidence = 0.87):
                    puzzleModel.grid[y][x] = "corr"
                    checked = True
                else:
                    puzzleModel.grid[y][x] = "empty"
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
                    if (puzzleModel.grid[startY][startX + x] == "empty"):
                        self.selectCell(self, puzzleModel, startX + x, startY)
                        if fillBool == False: 
                            puzzleModel.grid[startY][startX + x] = "mark"
                        print(puzzleModel.grid[startY][startX + x])
                else:
                    if (puzzleModel.grid[startY + x][startX] == "empty"):
                        self.selectCell(self, puzzleModel, startX, startY + x)  
                        if fillBool == False: 
                            puzzleModel.grid[startY + x][startX] = "mark"
                        print(puzzleModel.grid[startY + x][startX]) 

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
                    if fillBool == False: 
                        puzzleModel.grid[each, coord] = "mark"
                    print(puzzleModel.grid[each, coord])
            else:
                if (puzzleModel.grid[coord, each] == "empty"):
                    self.selectCell(self, puzzleModel, coord, each)
                    if fillBool == False: 
                        puzzleModel.grid[coord, each] = "mark"
                    print(puzzleModel.grid[coord, each])

    def phaseOne(self, puzzleModel):
        print("Phase One")
        #input(str("hit enter"))
        #Check for "0" clues
        clueLoc = np.where(puzzleModel.workingClues == 0)
        for i in range(len(clueLoc[0])):
            rowFill = clueLoc[1][i] == 0
            self.fillFullRow(self, puzzleModel, clueLoc[0][i], rowFill, False)
            puzzleModel.workingClues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]] = 0
            
        #Check for clues that are equal to size
        clueLoc = np.where(puzzleModel.workingClues == puzzleModel.size)
        for i in range(len(clueLoc[0])):
            rowFill = clueLoc[1][i] == 0
            self.fillFullRow(self, puzzleModel, clueLoc[0][i], rowFill, True)
            puzzleModel.workingClues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]] = 0

        #change the empty and 10 values to 0, now that all 0 clues have been handled
        puzzleModel.workingClues = np.where(abs(puzzleModel.workingClues) >= puzzleModel.size, 0, puzzleModel.workingClues)

        #Clues that completely fill a row
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

    def phaseTwo(self, puzzleModel):
        #check for clues that have been filled.
        print("Phase Two")
        #input(str("hit enter"))
        
        print("Mark any empty cells where all clues have been fulfilled")
        input()
        #for each column
        for x in range(puzzleModel.size):
            corrCount = 0
            #count the number of correct cells in the column
            for y in range(puzzleModel.size):
                if puzzleModel.grid[y][x] == "corr":
                    corrCount += 1
            #if the number of correct clues = the sum of the clues
            if corrCount == np.sum(np.where(abs(puzzleModel.clues[x][1]) <= puzzleModel.size, puzzleModel.clues[x][1], 0)):
                for y in range(puzzleModel.size):
                    if puzzleModel.grid[y][x] == "empty":
                        self.fillCells(self, puzzleModel, 1, x, y, False, False)
                np.where(puzzleModel.workingClues[x][1] != 0, 0, 0)
        #same for rows
        for y in range(puzzleModel.size):
            corrCount = 0
            #count the number of correct cells in the column
            for x in range(puzzleModel.size):
                if puzzleModel.grid[y][x] == "corr":
                    corrCount += 1
            #if the number of correct clues = the sum of the clues
            if corrCount == np.sum(np.where(abs(puzzleModel.clues[y][0]) <= puzzleModel.size, puzzleModel.clues[y][1], 0)):
                for y in range(puzzleModel.size):
                    if puzzleModel.grid[y][x] == "empty":
                        self.fillCells(self, puzzleModel, 1, x, y, False, False)
                np.where(puzzleModel.workingClues[y][0] != 0, 0, 0)
        
        print("where the largest clue in a line is fulfilled")
        input()
        #get highest clue in the line
        #check for adj correct cells
        #if they are equal to the highest clue, mark a cell either side.

        #for each row
        for y in range(puzzleModel.size):
            #again, skip any all 0 line
            if np.any(puzzleModel.workingClues[y][0] != 0):
                #find the highest clue
                highClue = np.amax(puzzleModel.workingClues[y][0])
                #find the highest number of correct adjactent cells
                highest = 0
                curr = 0
                start = 0
                for x in range(puzzleModel.size):
                    if puzzleModel.grid[y][x] == "corr":
                        if curr == 0:
                            start = x
                        curr += 1
                    if curr >= highest:
                        highest = curr
                if highClue == highest:
                    self.fillCells(self, puzzleModel, 1, start - 1, y, True, False)
                    self.fillCells(self, puzzleModel, 1, start + highest, y, True, False)
        #for each col
        for x in range(puzzleModel.size):
            #skip all 0 lines
            if np.any(puzzleModel.workingClues[x][1] != 0):
                #find the highest clue
                highClue = np.amax(puzzleModel.workingClues[x][1])
                #find the highest number of correct adjacent cells
                highest = 0
                curr = 0
                start = 0
                for y in range(puzzleModel.size):
                    if puzzleModel.grid[y][x] == "corr":
                        if curr == 0:
                            start = y
                        curr += 1
                    if curr >= highest:
                        highest = curr
                if highClue == highest:
                    self.fillCells(self, puzzleModel, 1, x, start - 1, False, False)
                    self.fillCells(self, puzzleModel, 1, x, start + highest, False, False)

        print("Where there is a correct cell within (clue[0]) range of the edge, or one beyond that.")
        input()
        #for each set of clues
        #rows first
        for line in range(puzzleModel.size):
            #skipping lines that are already satisfied
            if np.any(puzzleModel.workingClues[line][0] != 0):
                #get the position of the first valid clue in the line
                firstClue = (np.where(puzzleModel.clues[line][0] <= puzzleModel.size)[0][0])
                #check for a correct cell in the grid within the range of the first clue of the edge
                if np.any((puzzleModel.grid[:][line])[:puzzleModel.workingClues[line][0][firstClue]] == "corr"):
                    #if found, fill from that filled cell to the clue - 1
                    self.fillCells(self, puzzleModel, puzzleModel.workingClues[line][0][firstClue] - np.where(puzzleModel.grid[:][line] == "corr")[0][0] - 1, np.where(puzzleModel.grid[:][line] == "corr")[0][0], line, True, True)
                #same for again for the last clue
                if np.any((puzzleModel.grid[:][line])[puzzleModel.size - puzzleModel.workingClues[line][0][-1]:] == "corr"):
                    self.fillCells(self, puzzleModel, puzzleModel.workingClues[line][0][-1] - (puzzleModel.size - np.where(puzzleModel.grid[:][line] == "corr")[0][-1]), puzzleModel.size - puzzleModel.workingClues[line][0][-1], line, True, True)
                
                #if the cell at the clue value is filled, then the edge cell can be marked
                if puzzleModel.grid[line][puzzleModel.workingClues[line][0][firstClue]] == "corr":
                    self.fillCells(self, puzzleModel, 1, 0, line, True, False)
                if puzzleModel.grid[line][puzzleModel.size - int(puzzleModel.workingClues[line][0][-1]) - 1] == "corr":
                    self.fillCells(self, puzzleModel, 1, puzzleModel.size - 1, line, True, False)
            #Do the same for col
            if np.any(puzzleModel.workingClues[line][1] != 0):
                firstClue = (np.where(puzzleModel.clues[line][1] <= puzzleModel.size)[0][0])
                #check for a correct cell in the grid within the range of first clue of the edge
                if np.any((puzzleModel.grid[:, line])[:puzzleModel.workingClues[line][1][firstClue]] == "corr"):
                    #if found, fill from that filled cell to the clue - 1
                    self.fillCells(self, puzzleModel, puzzleModel.workingClues[line][1][firstClue] - np.where(puzzleModel.grid[:, line] == "corr")[0][0], line, np.where(puzzleModel.grid[:, line] == "corr")[0][0], False, True)
                #same again for the last clue
                if np.any((puzzleModel.grid[:, line])[puzzleModel.size - puzzleModel.workingClues[line][1][-1]:] == "corr"):
                    self.fillCells(self, puzzleModel, puzzleModel.workingClues[line][1][-1] - (puzzleModel.size - np.where(puzzleModel.grid[:, line] == "corr")[0][-1]), line, puzzleModel.size - puzzleModel.workingClues[line][1][-1], False, True)
                #if the cell at the clue value is filled, then the edge cell can be marked
                if puzzleModel.grid[:, line][puzzleModel.workingClues[line][1][firstClue]] == "corr":
                    self.fillCells(self, puzzleModel, 1, line, 0, False, False)
                if puzzleModel.grid[:, line][puzzleModel.size - puzzleModel.workingClues[line][1][-1] - 1] == "corr":
                    self.fillCells(self, puzzleModel, 1, line, puzzleModel.size - 1, False, False)

    def phaseThree(self, puzzleModel):
        print("Phase Three")
        input(str("hit enter"))
        print("+1 clues that fill gap")

        #identify gaps
        #for each column
        for x in range(puzzleModel.size):
            gapCount = 0
            gapSize = [0]
            gapLoc = [0]
            #find the gap by iterating through to the first empty cell
            for y in range(puzzleModel.size):
                if puzzleModel.grid[y][x] == "empty":
                    if gapSize[gapCount] == 0:
                        #storing that coord as the first cell in the gap
                        gapLoc[gapCount] = y
                    #counting the no. of empty cells
                    gapSize[gapCount] += 1
                #break at the end of the gap
                elif (puzzleModel.grid[y][x] != "empty") & (gapSize[gapCount] != 0):
                    gapCount += 1
                    gapSize.append(0)
                    gapLoc.append(0)
                #continue through to the end of the line
            print(gapSize)

            smallClue = 0
            largeClue = 0
            #with a gap found
            #if the gap is the same size as the largest clue
            if puzzleModel.workingClues[x][1][puzzleModel.workingClues[x][1] > 0].size > 0:
                smallClue = np.min(puzzleModel.workingClues[x][1][puzzleModel.workingClues[x][1] > 0])
                largeClue = np.max(puzzleModel.workingClues[x][1][puzzleModel.workingClues[x][1] > 0])
                print("{0}, {1}", smallClue, largeClue)

            #for each of the gaps
            for i in range(len(gapSize)):
                #determine if the gap ends with a filled cell - this is important for if a clue might be part complete
                if gapSize[0] > 0:
                    fillable = True
                        
                    #that doesn't work - just change it to check for -1 or gridsize
                    print(gapLoc[i]-1)
                    print(gapLoc[i] + gapSize[i])
                    print(gapLoc[i] + gapSize[i] == puzzleModel.size)

                    #if (gapLoc[i] - 1 == -1) or gapLoc[i] + gapSize[i] == puzzleModel.size):
                    #    print("edge")
                    #else:
                    #    if puzzleModel.grid[gapLoc[i] - 1][x] == "corr" or puzzleModel.grid[gapLoc[i] + gapSize[i]][x] == "corr":
                    #        fillable = False
                    
                    if (gapLoc[i] -1 > -1):
                        if (puzzleModel.grid[gapLoc[i] - 1][x] == "corr"):
                            fillable = False #(served by a different case)

                    if (gapLoc[i] + gapSize[i] < puzzleModel.size):                  
                        if (puzzleModel.grid[gapLoc[i] + gapSize[i]][x] == "corr"):
                            fillable = False #(served by a different case)

                    if fillable:
                        print("fillable")
                        #A gap bordered by anything other than correct cells can be treated as if it were an empty row
                        #look at the clues
                        #if there is only one clue,
                        #if the clue is smaller than the size 


            #Where the largest clue is equal to the largest unique gap, it can be filled
            if (largeClue == np.max(gapSize)) & largeClue > 0:
                print("large match")
                print(gapLoc[np.where(gapSize == largeClue)[0][0]])
            #    print(np.where(gapSize == largeClue))
            #    self.fillCells(self, puzzleModel, largeClue, x, gapLoc[np.where(gapSize == largeClue)[0][0]], False, True)


            #gapCount = len(np.where(puzzleModel.workingClues[x][1] > 0)[0]) - 1
            #print((sum(puzzleModel.workingClues[x][1]) + gapCount))
            #if ((sum(puzzleModel.workingClues[x][1]) + gapCount) == gapSize):
                #cellplace tracks what cell in the line the bot has gotten to
            #    cellPlace = startY
            #    for k in range(len(puzzleModel.workingClues[x][1])):
            #        if puzzleModel.workingClues[x][1][k] != 0:
            #            self.fillCells(self, puzzleModel, puzzleModel.workingClues[x][1][k], startX, cellPlace, False, True)
            #            cellPlace += puzzleModel.workingClues[x][1][k]
            #            if cellPlace != gapSize:
            #                self.fillCells(self, puzzleModel, 1, startX, cellPlace, False, False)
            #                cellPlace += 1
            #                print(cellPlace)
            #            puzzleModel.workingClues[x][1][k] = 0
            
            #If a clue in this row is more than half the size of the given gap
            #print("half clues")
            #clueLoc = np.where(puzzleModel.workingClues > gapSize / 2)
            #print(clueLoc[0])
            #for i in range(len(clueLoc[0])):
                #fillNum indicates how many either side should be missed
                #fillNum = gapSize - puzzleModel.workingClues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]]
                #if clueLoc[1] == 0, its a row clue
                #if clueLoc[1][i] == 0:
                #    self.fillCells(self, puzzleModel, gapSize - (fillNum * 2), fillNum, clueLoc[0][i], clueLoc[1][i] == 0, True)
                    #clue should not be set to 0 as the full clue has not been fulfilled
                #else:
                #    self.fillCells(self, puzzleModel, gapSize - (fillNum * 2), clueLoc[0][i], fillNum, clueLoc[1][i] == 0, True)
                    #clue should not be set to 0 as the full clue has not been fulfilled

        #for each row
        #for y in range(puzzleModel.size):
        #    gapSize = 0
            #find the gap by iterating through to the first empty cell
        #    for x in range(puzzleModel.size):
        #        if puzzleModel.grid[y][x] == "empty":
        #            if gapSize == 0:
                        #storing that coord as the first cell in the gap
        #                startX = x
        #                startY = y
                    #counting the no. of empty cells
        #            gapSize += 1
                #until another filled cell
        #        elif (puzzleModel.grid[y][x] != "empty") & (gapSize != 0):
        #            break
                #continue through to the end of the line
                #if there is another empty cell, break and move to the next line.

            #with a gap found
        #    puzzleModel.workingClues[y][0]
        #    gapCount = len(np.where(puzzleModel.workingClues[y][0] > 0)[0]) - 1
        #    if ((sum(puzzleModel.workingClues[y][0]) + gapCount) == gapSize):
                #cellplace tracks what cell in the line the bot has gotten to
        #        cellPlace = startX
        #        for k in range(len(puzzleModel.workingClues[y][0])):
        #            if puzzleModel.workingClues[y][0][k] != 0:
        #                self.fillCells(self, puzzleModel, puzzleModel.workingClues[y][0][k], cellPlace, startY, True, True)
        #                cellPlace += puzzleModel.workingClues[y][0][k]
        #                if cellPlace != gapSize:
        #                    self.fillCells(self, puzzleModel, 1, cellPlace, startY, True, False)
        #                    cellPlace += 1
        #                puzzleModel.workingClues[y][0][k] = 0

        #what about if there are two gaps
        #the presence of more than one gap gives several possibilities.
        #1) that only one gap had any correct cells
        #2) that correct cells are spread across the gap.
        #for the moment - only do this where only one gap exists.

        ###
        ###Old code that needs to be rewritten for a gap of any size rather than a full line
        ###
        
        #A clue is more than half the size
        #clueLoc = np.where(puzzleModel.workingClues > (puzzleModel.size / 2))
        #print("half clues")
        #print(clueLoc[0])
        #for i in range(len(clueLoc[0])):
            #fillNum indicates how many either side should be missed
        #    fillNum = puzzleModel.size - puzzleModel.workingClues[clueLoc[0][i]][clueLoc[1][i]][clueLoc[2][i]]
            #if clueLoc[1] == 0, its a row clue
        #    if clueLoc[1][i] == 0:
        #        self.fillCells(self, puzzleModel, puzzleModel.size - (fillNum * 2), fillNum, clueLoc[0][i], clueLoc[1][i] == 0, True)
                #clue should not be set to 0 as the full clue has not been fulfilled
        #    else:
        #        self.fillCells(self, puzzleModel, puzzleModel.size - (fillNum * 2), clueLoc[0][i], fillNum, clueLoc[1][i] == 0, True)
                #clue should not be set to 0 as the full clue has not been fulfilled
                
        #check for cells on the edges filled
        #for x in range(puzzleModel.size):
        #    if puzzleModel.grid[x][0] == "corr":
                #read the first clue for that 
        #        print(x)
        #        clue = puzzleModel.workingClues[x][1][0]
        #        self.fillCells(self, puzzleModel, clue, x, 0, False, True)
        #        self.fillCells(self, puzzleModel, 1, x, clue, False, False)
            
        #    if puzzleModel.grid[x][-1] == "corr":
        #        print(x)
        #        clue = puzzleModel.workingClues[x][1][-1]
        #        self.fillCells(self, puzzleModel, clue, x, puzzleModel.size - clue, False, True)
        #        self.fillCells(self, puzzleModel, 1, x, puzzleModel.size - clue - 1, False, False)

        #for y in range(puzzleModel.size):
        #    if puzzleModel.grid[0][y] == "corr":
        #        print(y)
                #read the first clue for that 
        #        clue = puzzleModel.workingClues[y][0][0]
        #        self.fillCells(self, puzzleModel, clue, 0, y, True, True)
        #        self.fillCells(self, puzzleModel, 1, clue, y, True, False)
            
        #    if puzzleModel.grid[-1][y] == "corr":
        #        print(y)
        #        clue = puzzleModel.workingClues[y][0][-1]
        #        self.fillCells(self, puzzleModel, clue, puzzleModel.size - clue, y, True, True)
        #        self.fillCells(self, puzzleModel, 1, puzzleModel.size - clue - 1, y, True, False)

    def phaseFour(self, puzzleModel):
        print("Phase Four")
        input(str("hit enter"))
        #if theres no clear next step, pick a random empty cell.
        emptyCells = np.where(puzzleModel.grid == "empty")
        #select random from range eptyCells
        selection = np.random.choice(emptyCells[0])
        self.fillCells(self, puzzleModel, 1, emptyCells[0][selection], emptyCells[1][selection], False, True)
    
    def solve(self, puzzleModel):
        self.phaseOne(self, puzzleModel)
        self.phaseTwo(self, puzzleModel)
        self.phaseTwo(self, puzzleModel)
        self.phaseThree(self, puzzleModel)
        #self.phaseFour(self, puzzleModel)

def main():
    model = puzzleModel()
    puzzleSolver.solve(puzzleSolver, model)
    print("end")
    
if __name__ == "__main__":
    main()