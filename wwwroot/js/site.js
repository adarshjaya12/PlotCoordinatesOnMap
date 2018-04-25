// Write your JavaScript code.
function initMap() {
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 3,
        center: { lat: 0, lng: -180 },
        mapTypeId: 'terrain'
    });

    var circle = {
        path: google.maps.SymbolPath.CIRCLE,
        fillColor: 'red',
        fillOpacity: .4,
        scale: 2,
        strokeColor: 'white',
        strokeWeight: 0.1
    };

    var flightPlanCoordinates = window.bootstrappedData;

    var marker, i;

    for (i = 0; i < flightPlanCoordinates.length; i++) {
        marker = new google.maps.Marker({
            position: new google.maps.LatLng(flightPlanCoordinates[i].lat, flightPlanCoordinates[i].lng),
            map: map,
            icon: circle
        });

        google.maps.event.addListener(marker, 'click', (function (marker, i) {
            return function () {
                infowindow.setContent(flightPlanCoordinates[i].lat);
                infowindow.open(map, marker);
            }
        })(marker, i));
    }
    //var flightPath = new google.maps.Polyline({
    //    path: flightPlanCoordinates,
    //    geodesic: true,
    //    strokeColor: '#FF0000',
    //    strokeOpacity: 1.0,
    //    strokeWeight: 2
    //});

    //flightPath.setMap(map);
}