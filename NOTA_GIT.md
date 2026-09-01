# 📝 Nota de Uso: Git para Unity (PC Trabajo ↔ Casa)

Este proyecto ya está inicializado como repositorio local de Git.

## ⚠️ REGLA DE ORO (Leer antes de nada)
**CIERRA UNITY COMPLETAMENTE** antes de ejecutar cualquier `git pull` o `git push`. Si Unity está abierto, bloqueará archivos y causará errores.

---

## 1️⃣ Crear repositorio en GitHub (Una sola vez)
1. Ve a GitHub → New repository.
2. Nombre: `tetris-unity-game` (Privado o Público).
3. **NO** marques "Add README", ".gitignore" o "license". Déjalo vacío.
4. Crea el repositorio y copia la URL (ej: `https://github.com/TU_USUARIO/tetris-unity-game.git`).

## 2️⃣ Subir el proyecto desde la PC del Trabajo (Una sola vez)
Abre la terminal en la carpeta del proyecto (`C:\Users\smith\Tetris`) y ejecuta:
```bash
git add .
git commit -m "Primer commit del proyecto"
git branch -M main
git remote add origin https://github.com/TU_USUARIO/tetris-unity-game.git
git push -u origin main
(Si pide credenciales, usa tu usuario y tu Personal Access Token de GitHub).
3️⃣ Descargar el proyecto en la PC de Casa (Una sola vez)
bash

cd C:\  # O la carpeta donde quieras guardarlo
git clone https://github.com/TU_USUARIO/tetris-unity-game.git

Luego, abre Unity Hub → Add → Selecciona la carpeta clonada. Unity regenerará la carpeta Library automáticamente.
🔄 4️⃣ Flujo Diario de Trabajo
📤 Al TERMINAR de trabajar (Subir cambios):
Cierra Unity.
En la terminal:
bash

   git add .
   git commit -m "Descripción breve del cambio"
   git push origin main
   
📥 Al EMPEZAR a trabajar en la otra PC (Bajar cambios):
Cierra Unity (si estaba abierto).
En la terminal:
bash

   git pull origin main

Abre Unity.
5️⃣ Qué SÍ y qué NO se sube a GitHub
El archivo .gitignore ya se encarga de esto, pero es bueno saberlo:
✅ SÍ se sube: Assets/, ProjectSettings/, Packages/, .gitignore.
❌ NO se sube (se ignoran): Library/, Temp/, Logs/, UserSettings/, *.sln, *.csproj. (Estos se regeneran solos al abrir Unity).
6️⃣ Comandos Útiles
Ver estado: git status
Ver historial: git log --oneline -n 5
Si algo se rompe al hacer pull: git merge --abort
7️⃣ Opción Visual (VS Code)
Si no quieres usar la terminal, puedes usar la pestaña "Source Control" (icono de ramas a la izquierda en VS Code):
Escribe el mensaje en el cuadro de texto.
Clic en "Commit" (✓).
Clic en "Sync Changes" o "Push" (↑).
(Recuerda: ¡Con Unity cerrado!)
