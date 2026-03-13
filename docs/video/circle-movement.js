console.log("Hello world!");

// Hardcoded values from html
const startX = 30;
const startY = 50;
const controlX = 180;
const controlY = 350;
const endX = 330;
const endY = 50;

// Hardcoded values from html
const strokeWidth = 4;
const circleRadius = 20;

const circle = document.getElementById("circle-to-animate");

var t = 0.8;

var frameLength = 20;

var currentLength = 0;
const animationLength = 2000;

var animationInterval = setInterval(animatedSVG, frameLength);

function animatedSVG(){
    currentLength += frameLength / animationLength;

    if (currentLength >= 1){
        console.log("Ending animation");
        clearInterval(animationInterval);
        currentLength = 1;
    }

    // Get the coordinates of a point on the curve
    var [targetX, targetY] = bezierPoint(startX, controlX, endX, startY, controlY, endY, currentLength);

    // Get the normal vector
    var [normalX, normalY] = bezierNormal(startX, controlX, endX, startY, controlY, endY, currentLength);

    // Invert the normal vector to make it point the opposite direction
    normalX *= -1;
    normalY *= -1;

    // Make scaled offsets in the direction of the normal vector
    var offsetX = normalX * (circleRadius + strokeWidth);
    var offsetY = normalY * (circleRadius + strokeWidth);

    // Offset the target
    targetX += offsetX;
    targetY += offsetY;

    // Update circle position
    circle.setAttribute("cx", targetX);
    circle.setAttribute("cy", targetY);
}

// Returns a point on a quadratic bezier curve
function pointOnBezier(p0, p1, p2, t) {
  return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
}

// Returns an array containing two dimensional position of a point on a quadratic bezier curve
function bezierPoint(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var pointA = pointOnBezier(p0A, p1A, p2A, t);
  var pointB = pointOnBezier(p0B, p1B, p2B, t);
  return [ pointA, pointB ];
}

// Returns the velocity for one dimension of a quadratic bezier curve
function velocityOfBezier(p0, p1, p2, t) {
  return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
}

// Returns an array containing two dimensional velocity of a point on a quadratic bezier curve
function bezierVelocity(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var velocityA = velocityOfBezier(p0A, p1A, p2A, t);
  var velocityB = velocityOfBezier(p0B, p1B, p2B, t);
  return [ velocityA, velocityB ];
}

// Returns the speed for a two dimensional quadratic bezier curve
function bezierSpeed(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var [velocityA, velocityB] = bezierVelocity(p0A, p1A, p2A, p0B, p1B, p2B, t);
  return Math.sqrt(velocityA * velocityA + velocityB * velocityB);
}

// Returns the unit tangent vector for a two dimensional quadratic bezier curve
function bezierTangentVector(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var [velocityA, velocityB] = bezierVelocity(p0A, p1A, p2A, p0B, p1B, p2B, t);
  var speed = bezierSpeed(p0A, p1A, p2A, p0B, p1B, p2B, t);
  return [ velocityA/speed, velocityB/speed ];
}

// Returns a three dimensional cross product
function crossProduct(a1, a2, a3, b1, b2, b3){
  var s1 = a2*b3-a3*b2;
  var s2 = a3*b1-a1*b3;
  var s3 = a1*b2-a2*b1;
  return [ s1, s2, s3 ];
}

// Returns the normal vector for a two dimensional quadratic bezier curve
function bezierNormal(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var [ tangentA, tangentB ] = bezierTangentVector(p0A, p1A, p2A, p0B, p1B, p2B, t);
  var [ cross1, cross2, cross3 ] = crossProduct(0, 0, 1, tangentA, tangentB, 0);
  return [ cross1, cross2 ];
}

