# Purr2
Home automation hub wrapper

This project is a C# wrapper for the Hubitat Elevation home automation hub. Specifically the code in this repository wraps the MakerAPI api with a monitor that listens over a websocket for events on the Hubitat hub. These events are forwarded to a C# worker service to be handled as the user sees fit.
