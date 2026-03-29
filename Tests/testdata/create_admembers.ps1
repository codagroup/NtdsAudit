Import-Module ActiveDirectory

$numComputers = 100
$numUsers = 100
$defaultPassword = "Pa55w0rd!"

# Data Sources
$ous = @(
    "TestLabUsers",
    "TestLabComputers",
    "Marketing",
    "HR",
    "IT",
    "Accounting",
    "Management",
    "PR",
    "Operations",
    "Legal", 
    "Purchasing"
)

$groups = @(
    "Printers",
    "Folders"
)

$firstNames = @(
    "Rebecca",
    "Justine",
    "Kerr",
    "Aubrey",
    "Roland",
    "Tucker",
    "Laren",
    "Brent",
    "Hulda",
    "Whitney",
    "Wilfried",
    "Bradley",
    "Jerald",
    "Pascoe",
    "Holly",
    "Chet",
    "Braden",
    "Warren",
    "Bret",
    "Charlton",
    "Gregory",
    "Doreen",
    "Jerry",
    "Gwen",
    "Demi",
    "Joey",
    "Cowden",
    "Savannah",
    "Emery",
    "Earlene",
    "Jensen",
    "Adie",
    "Maxwell",
    "Violet",
    "Elaine",
    "Melinda",
    "Maddox",
    "Gladys",
    "Jemma",
    "Sue",
    "Percy",
    "Caden",
    "Kitty",
    "Mindi",
    "Mindy",
    "Trent",
    "Hazel",
    "Jethro",
    "Jade",
    "Molly",
    "Hastings",
    "Dennis",
    "Sydney",
    "Tim",
    "Jerrold",
    "Charles",
    "Diamond",
    "Ruby",
    "Timothy",
    "Greig",
    "Ivory",
    "Jenny",
    "Sheridan",
    "Travis",
    "Rosie",
    "Leanne",
    "Melville",
    "Kate",
    "Stewart",
    "Barry",
    "Ellis",
    "Reynold",
    "Evan",
    "Jill",
    "Sharon",
    "Georgiana",
    "Pippa",
    "Pamela",
    "Henrietta",
    "Kylie",
    "Elfriede",
    "Lianne",
    "Angie",
    "Kendra",
    "Darnell",
    "Kimball",
    "Darlene",
    "Nancy",
    "Ashton",
    "Trudie",
    "Louise",
    "Tammy",
    "Heather",
    "Rupert",
    "Daris",
    "Carrington",
    "Shiloh",
    "John Baptist",
    "Heston",
    "Lawrence",
    "Jodie",
    "Woodrow",
    "Irene",
    "Marlene",
    "Rosalie",
    "Gabriel",
    "Barnes",
    "Osbert",
    "Christopher",
    "Raleigh",
    "Jaylon",
    "Carlyle",
    "Dustin",
    "Alberic",
    "Timmy",
    "Burdine",
    "Tracy",
    "Harley",
    "Rhoda",
    "Gene",
    "Nigel",
    "Vicary",
    "Emory",
    "Cadence",
    "Maud",
    "Deb",
    "Chay",
    "Stacy",
    "Mort",
    "Scarlett",
    "Jean",
    "Jackie",
    "Kristy",
    "Helton",
    "Increase",
    "Velma",
    "Riley",
    "Bernadine",
    "Ashleigh",
    "Millicent",
    "Nelson",
    "Jigar",
    "Charlene",
    "Kathryn",
    "Poppy",
    "Darleen",
    "Keaton",
    "January",
    "India",
    "Basil",
    "Jessie",
    "Malford",
    "Sidney",
    "Brett",
    "Greenbury",
    "Walker",
    "Dorothy",
    "Bethany",
    "Jocelyn",
    "Rosaleen"
)

$lastNames = @(
    "Wood",
    "Brown",
    "Stokes",
    "Nicholson",
    "Alexander",
    "Ross",
    "Higgins",
    "Morris",
    "Fisher",
    "Andrews",
    "Moss",
    "Carroll",
    "Rose",
    "Buckley",
    "Adams",
    "Ferguson",
    "Briggs",
    "Dickinson",
    "Parry",
    "Ford",
    "Heath",
    "Singh",
    "Shah",
    "Woodward",
    "Mitchell",
    "Griffin",
    "Barnett",
    "Nash",
    "Henderson",
    "Lane",
    "Begum",
    "Hall",
    "Burrows",
    "O'Connor",
    "Berry",
    "Khan",
    "Hayes",
    "Byrne",
    "Perry",
    "Baker",
    "White",
    "Bond",
    "Armstrong",
    "Stevens",
    "West",
    "Macdonald",
    "Brooks",
    "Owen",
    "Matthews",
    "Cole",
    "Hamilton",
    "Stephens",
    "Moore",
    "Edwards",
    "Morgan",
    "Wright",
    "Knight",
    "Mann",
    "Hill",
    "Hammond",
    "Francis",
    "Atkinson",
    "Reynolds",
    "Robson",
    "Marshall",
    "Dunn",
    "Gallagher",
    "Ahmed",
    "Barber",
    "Hawkins",
    "Holmes",
    "Wilkinson",
    "Miah",
    "Rowe",
    "Lambert",
    "Goodwin",
    "Holland",
    "Townsend",
    "O'Brien",
    "Bird",
    "Lawson",
    "Lawrence",
    "Booth",
    "Lamb",
    "Osborne",
    "Ball",
    "McDonald",
    "Stephenson",
    "Pearce",
    "Gibbs",
    "Warren",
    "Holt",
    "Russell",
    "Patel",
    "Robertson",
    "Ali",
    "Taylor",
    "Frost",
    "Baxter",
    "Reed",
    "Gordon",
    "McCarthy",
    "Oliver",
    "Chapman",
    "Doyle",
    "Knowles",
    "Saunders",
    "Stone",
    "Curtis",
    "Hughes",
    "Lowe",
    "French",
    "Fletcher",
    "Anderson",
    "Todd",
    "Quinn",
    "Jenkins",
    "Lloyd",
    "Wheeler",
    "Marsh",
    "Griffiths",
    "Miles",
    "Lewis",
    "Kennedy",
    "Robinson",
    "O'Neill",
    "Mason",
    "Webster",
    "Clarke",
    "Ryan",
    "Woods",
    "Murphy",
    "Jackson",
    "Peters",
    "Walton",
    "Newton",
    "Harris",
    "Gregory",
    "Kelly",
    "Clark",
    "Harrison",
    "Day",
    "Shaw",
    "Hancock",
    "Wilson",
    "Howard",
    "Collins",
    "Kaur",
    "Freeman",
    "Powell",
    "Bishop",
    "Carter",
    "Page",
    "Bibi",
    "Bradley",
    "Ellis",
    "Campbell",
    "Cooke",
    "Davis",
    "Bell"
)

# Create OUs
$ous | ForEach-Object { 
    if ($_ -eq "TestLabUsers" -or $_ -eq "TestLabComputers") { 
        New-ADOrganizationalUnit -Name $_ -Path "DC=testlab,DC=local" 
    }
    else { 
        New-ADOrganizationalUnit -Name $_ -Path "OU=TestLabUsers,DC=testlab,DC=local"; 
        New-ADOrganizationalUnit -Name $_ -Path "OU=TestLabComputers,DC=testlab,DC=local"; 
    }
}

# Create Groups
foreach ($ou in $ous | Where-Object { $_ -ne "TestLabUsers" -and $_ -ne "TestLabComputers" }) {
    foreach ($group in $groups) {
        New-ADGroup -Name "$ou`_Users_$group" `
            -Path "OU=$ou,OU=TestLabUsers,DC=testlab,DC=local" `
            -GroupScope DomainLocal `
            -GroupCategory Security `
            -Description "$_`Description";
        New-ADGroup -Name "$ou`_Computers_$group" `
            -Path "OU=$ou,OU=TestLabComputers,DC=testlab,DC=local" `
            -GroupScope DomainLocal `
            -GroupCategory Security `
            -Description "$_`Description";
    }
}

# Create Computers
1..$numComputers | ForEach-Object { 
    $computerName = "WKS" + $_
    $group = $groups | Get-Random
    $ou = $ous | Where-Object { $_ -ne "TestLabUsers" -and $_ -ne "TestLabComputers" } | Get-Random

    New-ADComputer -Name $computerName `
        -SamAccountName $computerName  `
        -Path "OU=$ou,OU=TestLabComputers,DC=testlab,DC=local" 
    Add-ADGroupMember -Identity "$ou`_Computers_$group" -Members "$computerName`$"
}

# Create Users
1..$numUsers | ForEach-Object { 
    $firstName = $firstNames | Get-Random
    $lastName = $lastNames | Get-Random
    $group = $groups | Get-Random
    $ou = $ous | Where-Object { $_ -ne "TestLabUsers" -and $_ -ne "TestLabComputers" } | Get-Random
    $enabled = $_ % 7 -gt 0 # Disable every 7th user account
    $existingUser = Get-ADUser -Filter { samAccountName -like "$firstname.$lastname*" }
    if ($existingUser -eq $null) {
        # New user, so do nothing
    }
    elseif ($existingUser.GetType().Name -eq "ADUser" ) {
        # A user with this name already exists, so append ".1" to the last name
        $lastName = "$lastname.1"
    }
    else {
        # Multiple users with this name already exist, so append an increment to the last name
        $lastName = "$lastname.$($existingUser.Count)"
    }

    New-AdUser -SamAccountName "$firstName.$lastName" `
        -Path "OU=$ou,OU=TestLabUsers,DC=testlab,DC=local" `
        -GivenName $firstName `
        -Surname $lastName `
        -Name "$firstName $lastName" `
        -DisplayName "$firstName $lastName" `
        -UserPrincipalName "$firstName.$lastName@testlab.local" `
        -EmailAddress "$firstName.$lastName@testlab.local" `
        -AccountPassword (ConvertTo-SecureString $defaultPassword -AsPlainText -Force) `
        -Enabled $enabled `
        -ChangePasswordAtLogon $false
    Add-AdGroupMember -Identity "$ou`_Users_$group" -Members "$firstName.$lastName"
}