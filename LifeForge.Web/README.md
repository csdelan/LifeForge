# LifeForge Web - Character Sheet

A Blazor WebAssembly front-end application for displaying and managing your LifeForge character.

## Features

- **Character Overview**: View your character's name and basic information
- **Attributes Display**: Track Strength, Discipline, and Focus stats
- **Resource Management**: 
  - HP (Hit Points) bar with visual red progress indicator
  - MP (Mana Points) bar with visual blue progress indicator
  - Both show current/max values with percentage-based fills
- **Wealth Tracking**: Display all currencies (Gold, Karma, Design Workslot, etc.)
- **Class Progression**: 
  - View all active character classes
  - Track level and XP progress for each class
  - XP progress bars showing advancement to next level
- **Active Buffs**: Display all active buffs and debuffs affecting your character

## Running the Application

1. Navigate to the web project directory:
   ```bash
   cd LifeForge.Web
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Open your browser and navigate to the URL shown in the terminal (typically `https://localhost:5001` or `http://localhost:5000`)

4. Click on "Character Sheet" in the navigation menu to view your character

## Project Structure

- **Pages/CharacterSheet.razor**: Main character sheet component
- **Pages/CharacterSheet.razor.css**: Styling for the character sheet
- **Pages/Home.razor**: Updated landing page with LifeForge branding
- **Layout/NavMenu.razor**: Navigation menu with character sheet link

## Sample Data

The character sheet currently displays sample data to demonstrate the UI. To integrate with real character data:

1. Create a character service/state management solution
2. Replace the `CreateSampleCharacter()` method with actual data loading
3. Implement persistence (e.g., local storage, database, API calls)

## Design Notes

- Uses Bootstrap for responsive layout
- Custom CSS for progress bars with gradient fills
- Color-coded elements:
  - HP bars: Red gradient
  - MP bars: Blue gradient
  - XP bars: Orange/yellow gradient
  - Buffs: Green gradient
  - Debuffs: Red gradient
- Mobile-responsive design that adapts to smaller screens
