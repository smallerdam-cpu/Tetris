# Nota de uso: Git para trabajar el proyecto en casa y en la PC

Este proyecto ya se inicializó como repositorio local de Git.

## 1) Crear un repositorio en GitHub

1. Abre GitHub en tu navegador.
2. Click en `New repository`.
3. Pon un nombre, por ejemplo: `tetris-unity-game`.
4. Opcional: marca `Public` o `Private`.
5. No crees archivos adicionales desde GitHub, solo crea el repositorio vacío.
6. Copia la URL del repositorio. Ejemplo:
   `https://github.com/TU_USUARIO/tetris-unity-game.git`

## 2) Subir el proyecto desde esta PC

Abre `cmd` o PowerShell en la carpeta del proyecto y ejecuta:

```cmd
cd C:\Users\smith\Tetris
git add .
git commit -m "Primer commit del proyecto"
git branch -M main
git remote add origin https://github.com/TU_USUARIO/tetris-unity-game.git
git push -u origin main
```

Si te pide usuario y contraseña:
- Usa tu usuario de GitHub
- Si usas Token de acceso personal, usa ese token en lugar de tu contraseña

## 3) Trabajar desde otra PC en tu casa

### Opción A: Clonar el proyecto

```cmd
cd C:\
git clone https://github.com/TU_USUARIO/tetris-unity-game.git
cd tetris-unity-game
```

Abre la carpeta con VS Code y trabaja normalmente.

### Opción B: Abrir el proyecto ya clonado en VS Code

1. Abre VS Code.
2. File > Open Folder...
3. Selecciona la carpeta clonada.

## 4) Actualizar cambios desde tu casa

Cuando trabajes en tu casa y quieras subir cambios:

```cmd
cd C:\ruta\a\tetris-unity-game
git status
git add .
git commit -m "Actualizacion del juego"
git push origin main
```

## 5) Actualizar cambios desde tu PC principal

Cuando regreses a tu PC principal y quieras descargar lo último que hiciste en tu casa:

```cmd
cd C:\Users\smith\Tetris
git pull origin main
```

## 6) Ver el estado del repositorio

```cmd
git status
git log --oneline --decorate --graph -n 5
```

## 7) Flujo recomendado diario

### En tu PC principal:
```cmd
git pull origin main
```

### Cuando trabajes en tu casa:
```cmd
git add .
git commit -m "Descripcion del cambio"
git push origin main
```

### Cuando vuelvas a tu PC principal:
```cmd
git pull origin main
```

## 8) Recomendaciones importantes

- Haz commits pequeños y descriptivos.
- No subas `Library`, `Temp`, logs ni configuraciones de Unity si no es necesario.
- Se recomienda usar `.gitignore` que ya viene configurado en este proyecto.
- Si Unity te pide regenerar archivos, no te preocupes: los .csproj y la carpeta `Library` normalmente se ignoran.

## 9) Si quieres trabajar directamente desde VS Code

Puedes usar la pestaña de Git dentro de VS Code:

1. Abre el proyecto en VS Code.
2. Ve a la parte inferior izquierda a `Source Control`.
3. Haz `Commit` y luego `Push`.

O desde la terminal:

```cmd
git status
git add .
git commit -m "Cambio funcional"
git push origin main
```

## 10) Si quieres hacerlo todo desde CMD en Windows

### Subir desde tu PC:
```cmd
cd C:\Users\smith\Tetris
git add .
git commit -m "Primer subida"
git branch -M main
git remote add origin https://github.com/TU_USUARIO/tetris-unity-game.git
git push -u origin main
```

### Descargar en otra PC:
```cmd
cd C:\
git clone https://github.com/TU_USUARIO/tetris-unity-game.git
```

### Actualizar ambas computadoras:
```cmd
git pull origin main
```

## 11) Si quieres usar GitHub Desktop

También puedes abrir la carpeta con GitHub Desktop y hacer commit/push desde la interfaz, pero la línea de comandos te da más control y es más rápido para Unity.

## 12) Recomendación final

Para este tipo de proyecto Unity, lo ideal es:
- guardar la versión estable en GitHub
- hacer commits frecuentes
- no subir carpetas pesadas, solo scripts, Assets, ProjectSettings y .csproj
- usar `.gitignore` para evitar que `Library` y `Logs` llenen el repo

Si quieres, en el siguiente paso te puedo dejar un archivo `.bat` con comandos listos para que solo hagas doble clic y suba los cambios, o también una versión con comando para hacer pull/push rápidamente.
