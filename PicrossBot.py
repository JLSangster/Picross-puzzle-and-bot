#Picross bot rules based

import pyautogui

def selectCell(row, col):
    #work out what the xcoord and ycoord for that cell are
    pyautogui.click(xcoord, ycoord)

def toggleFill():
    #work out where on the screen the toggle is
    pyautogui.click(xcoord, ycoord)

def fillCells(fillNum, startX, startY, rowBool, fillBool):
    #check what mode the toggle is, and that it is what it needs to be
    for x in range(fillNum):
        if rowBool:
            selectCell(startX + x, startY)
        else:
            selectCell(startX, startY + x)

def markFullRow(row, col, rowBool):
    #check toggle is on tright mode
    #for length of whole grid
    #if rowBool
    #selectCell(row, x)
    #else
    #selectCell(x, col)