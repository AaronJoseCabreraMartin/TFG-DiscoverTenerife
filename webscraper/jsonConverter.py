import os #to explore folders and open files
import sys #to handle the comand parameters

## it receive the latitude and longitude coordinates and return the zone that that place is
def calculateZone(latitude, longitude):
  latitude = float(latitude)
  longitude = float(longitude)
  
  if latitude >= 28.40631 and longitude >= -16.93788 and latitude <= 28.60634 and longitude <= -16.11673:
    return "North"

  if latitude >= 28.14750 and longitude >= -16.93788 and latitude <= 28.40631 and longitude <= -16.67719:
    return "West"
  
  if latitude >= 28.14750 and longitude >= -16.67719 and latitude <= 28.40631 and longitude <= -16.53193:
    return "Center"
  
  if latitude >= 28.14750 and longitude >= -16.53193 and latitude <= 28.40631 and longitude <= -16.11673:
    return "East"

  if latitude >= 27.99321 and longitude >= -16.93788 and latitude <= 28.14750 and longitude <= -16.11673:
    return "South"

  return "Unknown"

## It receive the line of the comma separated value file and transforms it into a dictionary, it also adds the timesItHasBeenVisited_ and the zone_ properties
def transformToDictionary(string):
  splited = string.split(';')
  return {
    "address_" : splited[1],
    "imageLink_" : splited[4],
    "latitude_" : splited[2],
    "longitude_" : splited[3],
    "name_" : splited[0],
    "timesItHasBeenVisited_" : "0",
    "zone_" : calculateZone(splited[2],splited[3])
  }

## It takes a dictionary and transform it into a string that follows the JSON format
def fromDictionaryToJSON(dictionary):
  toReturn = "{ \"address_\": \"" + dictionary["address_"] + "\",\n"
  toReturn += "\"imageLink_\": \"" + dictionary["imageLink_"].strip() + "\",\n"
  toReturn += "\"latitude_\": \"" + dictionary["latitude_"] + "\",\n"
  toReturn += "\"longitude_\": \"" + dictionary["longitude_"] + "\",\n"
  toReturn += "\"name_\": \"" + dictionary["name_"] + "\",\n"
  toReturn += "\"timesItHasBeenVisited_\": \"" + dictionary["timesItHasBeenVisited_"] + "\",\n"
  toReturn += "\"zone_\": \"" + dictionary["zone_"] + "\"\n }"
  return toReturn

## Example of execution: python jsonConverter.py miradores playas parquesNaturales senderos charcos
if len(sys.argv) < 2:
  print("This script expects at least one folder for working. Not enough arguments.")
  exit()

typesOfSites = []
for i, arg in enumerate(sys.argv):
  if i != 0:
    typesOfSites.append(arg)

allPlaces = []
okPlaces = {}
toCheckPlaces = []

conversion = open("conversion.json", "w",encoding="utf-8")#it stores all the places that are OK
revisar = open("revisar.json", "w",encoding="utf-8")#it stores the places that are in two different types of sites, this places should be checked by hand

for typeOfSite in typesOfSites:
  okPlaces[typeOfSite] = []
  allPlaces.append((typeOfSite, []))
  file = open(os.path.join(os.path.join(os.getcwd(),typeOfSite), typeOfSite+".txt"), "r",encoding="utf-8")
  for line in file:
    try:
      #if the method index dont throw an exception means that that dictionary is already stored on that category, so skip it.
      allPlaces[-1][1].index(transformToDictionary(line))
    except:
      #if the exception was thrown, you have to add the dictionary conversion to the list.
      allPlaces[-1][1].append(transformToDictionary(line))

print("Detectando repetidos o fuera de la isla")
## Check all the types and all the places and it stores the places that are in to different types of places or the places that have an unkown zone
# in the toCheckPlaces list
for i in range(len(allPlaces)):
  for j in range(len(allPlaces)):
    for k in range(len(allPlaces[i][1])):
      for l in range(len(allPlaces[j][1])):
        # if its a different type place but there is two places that have the same name its duplicated so you have to check it manually 
        if k != l and allPlaces[i][1][k]["name_"] == allPlaces[j][1][l]["name_"]:
          try:
            toCheckPlaces.index(allPlaces[i][1][k])
          except:
            toCheckPlaces.append(allPlaces[i][1][k])#store only the place not the type (because we dont know the correct one)
      #if the zone of the place is Unknown we have to check it manually because it is probably outside tenerife
      if allPlaces[i][1][k]["zone_"] == "Unknown":
        toCheckPlaces.append(allPlaces[i][1][k])#store only the place not the type (because we dont know the correct one)


## Adds all the places that arent in the toCheckPlaces list on the okPlaces
for i in range(len(allPlaces)):
  for j in range(len(allPlaces[i][1])):
    try:
      #if the exception is not throw it means that the place is on the toCheckPlaces list so skip it
      toCheckPlaces.index(allPlaces[i][1][j])
    except:
      #if the exception was thrown, we have to keep that place so add it to the okPlaces list
      okPlaces[allPlaces[i][0]].append(allPlaces[i][1][j])

print("Exporting OK places")
conversion.write("{\"places_\":{\n")
for indexSite in range(len(typesOfSites)):
  typeOfSite = typesOfSites[indexSite]
  conversion.write("\"" + typeOfSite + "\": [\n")
  for index in range(len(okPlaces[typeOfSite])):
    conversion.write(fromDictionaryToJSON(okPlaces[typeOfSite][index]))
    if index + 1 != len(okPlaces[typeOfSite]):
      conversion.write(",")
  if indexSite + 1 != len(typesOfSites):
      conversion.write("],")
conversion.write("]\n}\n}")

print("Exporting places that you have to check manually")
revisar.write("{\"placesToCheck_\":[\n")
for index in range(len(toCheckPlaces)):
  revisar.write(fromDictionaryToJSON(toCheckPlaces[index]))
  if index + 1 != len(toCheckPlaces):
    revisar.write(",")
revisar.write("]\n}")

print("Process finished successfully")
file.close()
conversion.close()
revisar.close()