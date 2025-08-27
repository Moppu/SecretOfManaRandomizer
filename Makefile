LIBRARY_DIR = SoMRandomizerShared
GUI_DIR = SoMRandomizer

.PHONY: all
all: restore build

.PHONY: build
build: build-shared build-gui

.PHONY: build-shared
build-shared:
	@echo "Building library..."
	cd $(LIBRARY_DIR) && dotnet build

.PHONY: build-gui
build-gui:
	@echo "Building GUI..."
	cd $(GUI_DIR) && dotnet build

.PHONY: restore
restore:
	@echo "Restoring NuGet packages..."
	dotnet restore *.sln

.PHONY: clean
clean:
	@echo "Cleaning build artifacts..."
	cd $(LIBRARY_DIR) && dotnet clean && rm -rf ./bin ./obj
	cd $(GUI_DIR) && dotnet clean && rm -rf ./bin ./obj

.PHONY: rebuild
rebuild: clean restore build
