# Discover Tenerife - Final Work Proyect
## Author: _Aarón José Cabrera Martín_ - _alu0101101019_

### App Android
APK disponible en el siguiente [enlace](https://drive.google.com/file/d/1rSJFF9Nfca_R0dF-w-KhMR8tffNppb1G/view).

La aplicación de android ha sido desarrollada con Unity, necesitará abrir el proyecto con Unity si desea compilarlo usted mismo.

### Documentación del código fuente
Disponible en el siguiente [enlace](https://aaronjosecabreramartin.github.io/TFG-DiscoverTenerife/).

Para documentar el codigo fuente de la aplicación del proyecto he utilizado la herramienta gráfica de Doxygen.
El html resultado se encuentra en la carpeta _docs_.

### Web del proyecto
Disponible en el siguiente [enlace](https://discovertenerife-fd031.web.app/).

El código fuente de la página web se encuentra en la carpeta _discovertenerifeweb_.
Deberá ejecutar un `npm install`, luego para compilar y publicar la app en el hosting de firebase `npm run deploy`.

### Web scraper
El código fuente del web scraper se encuentra disponible en la carpeta _webscraper_.
Deberá ejecutar `python webscraper.py` para lanzar el webscraper, éste extraera información de google maps sobre puntos de interés que encajen en las categorías que tiene internamente definidas.
**Recuerde**: debe tener instalada y actualizada tanto la librería Selenium como el chromedriver o el driver que necesite para el navegador que utilice. Estos driver cambian constantemente por lo que suele ser un motivo de obtener errores.
Tras esperar a que el webscraper termine, el siguiente paso lógico sería ejecutar `python jsonConverter.py`, el cual limpiará aquellos lugares que estén repetidos dentro de la misma categoría, creará un JSON con todos los lugares que ha encontrado el web scraper y otro json con los lugares que habria que revisar a mano para decidir la categoría a la que pertenecen. 

### Cuestionario sobre el uso de la aplicación y de la web
Disponible en el siguiente [enlace](https://forms.gle/s8UD94vW4irVsJ3i6).

### Memoria del proyecto
La memoria del proyecto ha sido desarrollada con la herramienta Overleaf en el lenguaje de marcado Latex.
Tanto el código fuente como el pdf resultado se encuentran en la carpeta _memoria_.