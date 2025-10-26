# ?? KAPAL - Modern UI Update

## Perubahan Tampilan

Aplikasi KAPAL telah diupdate dengan **tampilan modern sidebar navigation** dan **warna pastel** yang lebih menarik.

---

## ?? Global Styles (App.xaml)

Semua button styles sudah didefinisikan sebagai **global resources** di `App.xaml`:

### Button Styles:
- **PrimaryButtonStyle** - Blue button (#3B82F6) untuk primary actions
- **SecondaryButtonStyle** - Gray button (#F1F5F9) untuk secondary actions
- **ActionButtonStyle** - Customizable colorful buttons (blue, green, dll)
- **DangerButtonStyle** - Red button (#FEE2E2) untuk delete actions

**Keuntungan:**
? Tidak perlu define styles di setiap page  
? Konsisten di seluruh aplikasi  
? Mudah maintenance - ubah sekali, semua page terpengaruh  
? Menghindari error "Cannot find resource"

---

## ?? Struktur Menu

### 1. **Dashboard** ??
**File:** `Views/DashboardPage.xaml`

**Fitur:**
- 3 Summary Cards:
  - Total Vessel ??? (Soft Blue)
  - Total Landing ? (Mint Green)
  - Total Species ?? (Soft Orange)
- 3 Analytics Charts:
  - Top Species by Total Weight (Pie Chart - colorful)
  - Vessel Count by Gear (Horizontal Bar Chart - blue)
  - Landings per Day - Last 14 Days (Line Chart - green)

---

### 2. **Input New** ?
**File:** `Views/InputNewPage.xaml`

**Fitur:**
- Button "Add New Vessel" (Soft Blue)
- Button "Add New Landing" (Mint Green - enabled after select vessel)
- List vessels dengan search functionality
- Search, Reset, Refresh buttons

---

### 3. **Data Vessel** ???
**File:** `Views/VesselDataPage.xaml`

**Fitur:**
- List semua vessel
- CRUD Operations:
  - ? **Add Vessel** (PrimaryButtonStyle - Blue)
  - ? **Edit Vessel** (SecondaryButtonStyle - Gray)
  - ? **Delete Vessel** (DangerButtonStyle - Red) with confirmation

---

### 4. **Data Landing** ?
**File:** `Views/LandingDataPage.xaml`

**Fitur:**
- List semua landing records
- **JOIN** dengan vessel name
- Columns: Landing ID, Vessel Name, Landed At, Notes
- Refresh button

**Model:** `LandingWithVessel` (JoinModels.cs)

---

### 5. **Data Catch** ??
**File:** `Views/CatchDataPage.xaml`

**Fitur:**
- List semua catch records
- **JOIN** dengan vessel name (via landing)
- Columns: Catch ID, Vessel Name, Species, Weight (kg)
- Refresh button

**Model:** `CatchWithVessel` (JoinModels.cs)

---

## ?? Design System

### Warna Pastel
```css
/* Background & Base Colors */
Background:     #F8FAFC (very light gray-blue)
Cards: #FFFFFF (white)
Borders:     #E2E8F0 (light gray)
Text Primary:   #0F172A (dark slate)
Text Secondary: #64748B (slate gray)
Hover:          #F1F5F9 (soft gray)

/* Button Colors */
Primary Blue:   #3B82F6 / #DBEAFE (background)
Secondary Gray: #F1F5F9 / #CBD5E1 (border)
Success Green:  #10B981 / #D1FAE5 (background)
Warning Orange: #F59E0B / #FFF4E6 (background)
Danger Red:     #DC2626 / #FEE2E2 (background)

/* Accent Colors */
Soft Pink:      #FFE5E7
Soft Purple:    #F3E5F5
Soft Yellow:    #FFF9C4
Soft Mint:      #E0F2F1
```

### Typography
```
Header 1:       32px Bold (#0F172A)
Header 2:       18px SemiBold (#1E293B)
Subtitle:       14px Regular (#64748B)
Summary Number: 28px Bold (#0F172A)
Body:        14px Regular (#475569)
```

### Spacing & Layout
```
Page Margin:    32px, 24px (top/bottom, left/right)
Card Padding:   20-24px
Card Margin:    16px between cards
Border Radius:  12px (cards), 8px (buttons), 6px (inputs)
Border:         1px solid #E2E8F0
```

### Components
```
DataGrid:
- HeadersVisibility: Column
- GridLinesVisibility: None
- AlternatingRowBackground: #F9FAFB
- Border: 1px #E2E8F0

Buttons:
- Height: 36px (secondary), Auto (primary/action)
- Padding: 16px 10px
- Font: 14px SemiBold
- Cursor: Hand pointer
- Hover: 0.85-0.9 opacity atau background change
```

---

## ?? File Structure

```
Kapal/
??? App.xaml          # ? GLOBAL STYLES HERE
??? App.xaml.cs
??? MainWindow.xaml (.cs)  # Sidebar navigation
??? Views/
?   ??? DashboardPage.xaml (.cs)      # 6 cards dashboard
?   ??? InputNewPage.xaml (.cs)  # Form input vessel/landing
?   ??? VesselDataPage.xaml (.cs)     # CRUD Vessel
?   ??? LandingDataPage.xaml (.cs)    # List landing + join vessel
?   ??? CatchDataPage.xaml (.cs)      # List catch + join vessel
?   ??? (old) HomePage.xaml (.cs)     # ? Deprecated - can delete
??? Models/
?   ??? Vessel.cs
?   ??? Landing.cs
?   ??? Catch.cs
?   ??? JoinModels.cs    # LandingWithVessel, CatchWithVessel
??? Services/
?   ??? Repositories.cs  # VesselRepo, LandingRepo, CatchRepo
??? Entities/
?   ??? VesselEntity.cs
?   ??? LandingEntity.cs
?   ??? CatchEntity.cs
??? Assets/
    ??? myKapal.png   # Hero background image
```

---

## ?? How to Run

1. **Build project:**
   ```bash
   dotnet build
   ```

2. **Run application:**
   ```bash
   dotnet run
   ```
   Or press **F5** in Visual Studio

3. **Default page:** Dashboard
4. **Navigate** using sidebar menu on the left

---

## ? Features Implemented

? Modern sidebar navigation with pastel gradient header  
? **Global button styles** in App.xaml (no more resource errors!)  
? Pastel color scheme throughout  
? Dashboard with 6 cards (3 summary + 3 charts)  
? Input New page with vessel list & search  
? Vessel CRUD operations (Add, Edit, Delete)  
? Landing list with JOIN vessel name  
? Catch list with JOIN vessel name  
? Responsive scrollable layout
? Clean card-based UI with rounded corners  
? Hover effects on buttons  
? Disabled state styling  
? Consistent DataGrid styling  

---

## ?? Technical Details

### Global Styles (App.xaml)
All button styles are defined globally to prevent `StaticResource` errors:

```xaml
<Application.Resources>
    <Style x:Key="PrimaryButtonStyle" TargetType="Button">
   <!-- Blue button for primary actions -->
    </Style>
    <Style x:Key="SecondaryButtonStyle" TargetType="Button">
        <!-- Gray button for secondary actions -->
    </Style>
    <Style x:Key="ActionButtonStyle" TargetType="Button">
        <!-- Customizable colored buttons -->
    </Style>
    <Style x:Key="DangerButtonStyle" TargetType="Button">
        <!-- Red button for delete actions -->
    </Style>
</Application.Resources>
```

### Sidebar Navigation
MainWindow uses a 2-column Grid layout:
- **Left:** 260px sidebar with navigation buttons
- **Right:** Frame for page content

All navigation is handled via Click events that call `RootFrame.Navigate()`.

### Data Models
**Join Models** for displaying combined data:
- `LandingWithVessel` - Landing data + Vessel Name
- `CatchWithVessel` - Catch data + Vessel Name (via Landing)

---

## ?? To Do (Future Implementation)

- [ ] **Add Vessel Dialog** - Modal form untuk input vessel baru
- [ ] **Add Landing Dialog** - Modal form untuk input landing baru
- [ ] **Edit Vessel Dialog** - Modal form untuk edit vessel
- [ ] **Add Catch functionality** - Input catch data
- [ ] **Advanced filtering** - Filter by date range, species, gear
- [ ] **Export to Excel/PDF** - Export data & charts
- [ ] **Dark mode toggle** - Switch tema gelap/terang
- [ ] **User authentication** - Login system
- [ ] **Data validation** - Form validation dengan error messages
- [ ] **Pagination** - Untuk list data yang banyak
- [ ] **Charts customization** - More chart options

---

## ?? Bug Fixes Applied

? **Fixed:** `Cannot find resource named 'PrimaryButtonStyle'`  
**Solution:** Moved all styles to `App.xaml` as global resources

? **Fixed:** `Cannot find resource named 'ActionButtonStyle'`  
**Solution:** Added ActionButtonStyle to global App.xaml

? **Fixed:** XAML parse exception on Page load  
**Solution:** Removed duplicate Page.Resources, use global only

? **Fixed:** Scroll not working on HomePage  
**Solution:** Removed fixed Height/Width from Page

? **Fixed:** BarSeries requires CategoryAxis on Y-axis  
**Solution:** Swapped axis positions for horizontal bar chart

---

## ?? Team
- **Ketua Kelompok**: Pradana Yahya Abdillah — `23/515259/TK/56625`  
- **Anggota 1**: Irfan Firdaus Isyfi — `23/520128/TK/57322`  
- **Anggota 2**: Muhammad Khaira Rahmadya Nauval — `23/521078/TK/57466`

---

## ?? Screenshots

### Dashboard
```
???????????????????????????????????????????????
? ?? KAPAL      ?
? Katalog Pelabuhan       ?
???????????????????????????????????????????????
? ?? Dashboard  ?  Dashboard         ?
? ? Input New  ?  ????????????????????       ?
? ??? Vessel     ?  [Total Vessel] [Landing]  ?
? ? Landing    ?  [Total Species]     ?
? ?? Catch      ?           ?
?      ?  [Pie Chart] [Bar] [Line]   ?
?      ?      ?
? KAPAL v1.0   ?           ?
???????????????????????????????????????????????
```

---

**Built with ?? using:**
- WPF .NET 8
- OxyPlot for charts
- Supabase for backend
- Modern pastel design

---

**Last Updated:** 2024
**Version:** 1.0.0
**Status:** ? Build Successful
