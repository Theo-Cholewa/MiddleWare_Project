# Let's Go Biking

## Description

A service that allows users to locate, pick up, drop off bicycles at partner stations for convenient and eco-friendly travel.

## Main features

- **Station finder** : Quickly locate available bike stations near you. </br>
- **Multi-Station contracts** : Drop the bike off at any partner station under the same contract.</br>
- **Integration with map** : Easily visualize routes and stations on an interactive map.

## Technologies used

- **Languaegs** : `JavaScript`, `C#`, `HTML`, `CSS` </br>
- **APIs** : `JCdecaux`, `adresse.data.gouv` `router.project-osrm`

## Installation

- **Clone the repository** :
    ```bash
    git clone https://github.com/Theo-Cholewa/MiddleWare_Project.git
    ```

- **Start the server** : </br>
    ***Location*** : /backend/Server/RoutingServer/RoutingServer/bin/Debug/RoutingServer.exe
    >Launch it as administrator

- **Start the proxy** : </br>
    ***Location*** : /backend/Server/RoutingServer/ProxyCache/bin/Debug/ProxyCache.exe
    >Launch it as administrator

- **Start the http server** : </br>
    ***Location*** : /frontend
    >Run the command : `http-server .`

## Utilisation

- **Enter two addresses** : departure &rarr; destination </br>
    - &rarr; By clicking the map </br>
    - &rarr; By typing it (take care to have the name of the city)

- The walking route will be displayed in **blue**, and the cycling route will be displayed in **red**.

## Authors

**Théo Cholewa, IT student, Polytech' Nice-Sophia**</br>
**Hugo Heilmann, IT student, Polytech' Nice-Sophia**
