#!/usr/bin/env python
# -*- coding: utf-8 -*-
import time #For wait
from numpy import random #For creating naturalwait method
import re #For regular expressions
from selenium import webdriver #For simulate navigation 
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
import os #For checking and creating folders

class ScrapearGMaps:
  
  def __init__(self, fileName = "data.txt", folder = ""):
    
    # Seleinum needs a chromediver.exe file for running the test on GoogleChorme.
    self.driver = webdriver.Chrome(executable_path=r"D:\Programas\UnityProyects\DiscoverTenerife\webscraper\chromedriver.exe") 
    
    # Folder where the extracted data will be stored
    self.folder = folder
    self.exportedFile = None
    self.changeOutputFile(fileName,folder)

    # List where it will store the name of the place that was found to be sure that it hasnt any duplicate info
    self.memory = []
  

  # gets geographical lat/long coordinates from the given url
  def get_geocoder(self, url_location): 
    try:
      # the regular expression search for a "!3d", one or zero "-", one or two digits, a ".", a serie of digits (between 3 to 7 digits)
      # a "!4d", one or zero - , one or two digits, a "." and a serie of digits (between 3 to 7 digits)
      coords = re.search(r"!3d-?\d\d?\.\d{4,8}!4d-?\d\d?\.\d{4,8}",
                      url_location).group()
      #quit the "!3d", qe keep the {number}"!4d"{number}
      coords = coords.split('!3d')[1]
      #split in a tuple that contains ({number},{number})
      return tuple(coords.split('!4d'))
    except (TypeError, AttributeError):
      print("no he encontrado las coordenadas")
      return ("", "")
      
  def get_name(self):
    try:
      #extract the headert-title element text, and quit all the weird characters using cleanString method
      return self.cleanString(str(self.driver.find_element_by_xpath("//h1[contains(@class,'header-title')]").text))
    except:
      print("no he encontrado el nombre")
      return ""

  def cleanString(self,toClean):
    # replace all the characters that match with the regular expresion with an empty string
    return re.sub("[\\'\".”/]","",toClean)
      
  def get_address(self):
    try:
      #extract the text of the address element
      return str(self.driver.find_element_by_css_selector("[data-item-id='address']").text)
    except:
      print("no he encontrado la direccion")
      return ""

  def get_image(self):
      try:
        #extract the link that points to the place's photo
        return self.driver.find_element_by_xpath("//button[contains(@aria-label,'Foto de')]").find_element(By.TAG_NAME, 'img').get_attribute('src')
      except:
        print("no he encontrado la imagen")
        return ""

  #change the route where the output file will be stored, if the folder isnt given it will use the current folder
  def changeOutputFile(self, newFile, newFolder = ""):
    if self.exportedFile is not None:
      self.exportedFile.close()
    self.memory = []
    self.folder = newFolder
    try:
      #with os.path.join we are sure that the route's structure is correct (using / or \ )
      if len(newFolder) > 0:
        self.exportedFile = open(os.path.join(newFolder, newFile), "a",encoding="utf-8")
        
      else:
        self.exportedFile = open(newFile, "a",encoding="utf-8")
    except:
      if len(newFolder) > 0:
        #if the Folder dont exists, create it
        if not os.path.exists(newFolder):
          os.mkdir(os.path.join(os.getcwd(),newFolder))
        
        self.exportedFile = open(os.path.join(os.path.join(os.getcwd(),newFolder), newFile), "w",encoding="utf-8")

      else:
        self.exportedFile = open(newFile, "w",encoding="utf-8")
    
  # search on the internet the given url and if the use conditions message poped up it acepts it 
  def search(self, url):
    self.driver.get(url)
    self.naturalWait(2)
    try:
      element = self.driver.find_element_by_xpath("//button[.//span[text()='Acepto']]")
      element.click()
    except:
      pass

  # it extracts the given number of elements of the google maps search. 
  # It stores the name, the geographical coords, the image and the address of the found place 
  def scrape(self, times = 20):
    try:
      self.naturalWait(3)          
      for i in range(0,times):
        # each 20 elements you have to go to the next page of places
        if i % 20 == 0 and i != 0: 
          element = self.driver.find_element(By.ID,"ppdPk-Ej1Yeb-LgbsSe-tJiF1e")
          self.naturalWait(1)
          element.click()
          self.naturalWait(6)
        
        print("aqui")
        # click on the correct place taking care of each page has only 20 elements
        place = self.searchNextElement(i%20)
        print("peto")

        place.click()
        
        self.naturalWait(3.5)
        
        # take the properties of the selected place
        name = self.get_name()
        address = self.get_address()
        coords = self.get_geocoder(self.driver.current_url)
        image = self.get_image()

        # if the selected place hasnt got any name, geographical coords or image, ignore that place.
        if name == "" or coords[0] == "" or coords[1] == "" or image == "":
          self.clickBackButton()
          self.naturalWait(2)
          continue
        
        # 28.593253, -16.082554 arriba derecha
        # 27.981354, -16.964207 abajo izquierda
        # if the selected place's georaphical coords are out of the island, ignore that place
        if float(coords[0]) < 28.593253 and float(coords[0]) > 27.981354 and float(coords[1]) < -16.082554 and float(coords[1]) > -16.964207:
          # if the place is already on the memory, ignore that place
          try:
            # index will throw an exception if the string of the name isnt on the memory
            self.memory.index(name)
          except:
            # if index throw the exception is because this place isnt on the memory, so we have to include it
            self.memory.append(name)
            print('Dentro de Tenerife:'+ str([ name, address, coords[0], coords[1] ]))
            self.writeOnExportedFile(name, coords, address, image)

        else:
          print('Fuera de Tenerife:'+ str([ name, address, coords[0], coords[1] ]))
        
        self.clickBackButton()
        self.naturalWait(3)
        
    except Exception as e:
        print(e)

    self.exportedFile.close()
  
  def writeOnExportedFile(self, name, coords, address, imageLink):
    # im using ; because the address could contain a ','
    if address == "":
      # some places dont have any address so I will use the geographical coords as an address
      self.exportedFile.write('' + name + '; ' + coords[0] + coords[1] + '; ' + coords[0] + '; ' + coords[1] + '; ' + imageLink + '\n') 
    else:
      self.exportedFile.write('' + name + '; ' + address + '; ' + coords[0] + '; ' + coords[1] + '; ' + imageLink + '\n')

  # waits for the given number of seconds plus a random number between 0 and 1
  def naturalWait(self,timeToSleep):
    time.sleep(timeToSleep+random.rand())
  
  # press the ArrowDown key until it found the given element (counting the elements)
  def searchNextElement(self,element):#hfpxzc V0h1Ob-haAclf 
    elementClass = "hfpxzc"
    while len(self.driver.find_elements(By.CLASS_NAME, elementClass)) <= element:
      self.driver.find_elements(By.CLASS_NAME, "section-scrollbox")[1].send_keys(Keys.ARROW_DOWN)
      self.naturalWait(0)
    #print(element,len(self.driver.find_elements(By.CLASS_NAME, elementClass)))
    self.naturalWait(1)
    return self.driver.find_elements(By.CLASS_NAME, elementClass)[element]

  # press the goback button 
  def clickBackButton(self):
    try:
      self.naturalWait(7)
      element = self.driver.find_element_by_xpath("//button[contains(@aria-label,'Atrás')]")
      self.naturalWait(3)
      element.click()
    except:
      try:
        self.naturalWait(7)
        element = self.driver.find_element_by_xpath("//button[contains(@aria-label,'Atrás')]")
        self.naturalWait(3)
        element.click()
      except:
        print("no he encontrado el boton Atrás")
  

query = "miradores tenerife"
url = "https://www.google.com/maps?q="+query.replace(" ", "+")+"&hl=es"

gmaps = ScrapearGMaps("miradores.txt", "miradores")

try:
  gmaps.search(url)
  gmaps.scrape(250)
except Exception as e:
  print(e)
'''

query = "playas"
url = "https://www.google.com/maps?q="+query.replace(" ", "+")+"&hl=es"
gmaps.changeOutputFile("playas.txt", "playas")
try:
  gmaps.search(url)
  gmaps.scrape(250)
except Exception as e:
  print(e)

#query = "senderos"
query = "zonas de senderismo"
url = "https://www.google.com/maps?q="+query.replace(" ", "+")+"&hl=es"
gmaps.changeOutputFile("senderos.txt", "senderos")
try:
  gmaps.search(url)
  gmaps.scrape(250)
except Exception as e:
  print(e)

query = "parques naturales"
url = "https://www.google.com/maps?q="+query.replace(" ", "+")+"&hl=es"
gmaps.changeOutputFile("parquesNaturales.txt", "parquesNaturales")
try:
  gmaps.search(url)
  gmaps.scrape(250)
except Exception as e:
  print(e)

query = "piscinas naturales"
url = "https://www.google.com/maps?q="+query.replace(" ", "+")+"&hl=es"
gmaps.changeOutputFile("charcos.txt", "charcos")
try:
  gmaps.search(url)
  gmaps.scrape(250)
except Exception as e:
  print(e)

query = "charcos"
url = "https://www.google.com/maps?q="+query.replace(" ", "+")+"&hl=es"
gmaps.changeOutputFile("charcos.txt", "charcos")
try:
  gmaps.search(url)
  gmaps.scrape(250)
except Exception as e:
  print(e)
'''

'''query = "sitios de interés"
url = "https://www.google.com/maps?q="+query.replace(" ", "+")+"&hl=es"
gmaps.changeOutputFile("mix.txt", "mix")
try:
  gmaps.search(url)
  gmaps.scrape(250)
except Exception as e:
  print(e)'''