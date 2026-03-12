// How many milliseconds should be between each frame of the animation
const frameLength = 20;
// Calls the animatedSVG function every "frameLength" milliseconds
var animationInterval = setInterval(animatedSVG, frameLength);

// How long the animation is total
const animationLength = 2000.0;
// How far along the animation we currently are
var currentLength = 0;
// Used to make animation reverse itself upon completion
var currentDirection = 1;

// For this example, to keep the code as simple as possible, I'll hardcode the bezier curve and circle values
const startX = 30;
const startY = 50;
const controlX = 180;
const controlY = 350;
const endX = 330;
const endY = 50;
const strokeWidth = 4;
const circleRadius = 20;

// This gets the circle by its id, so that we can change its attributes
const circle = document.getElementById("animated-circle");

// Updates position of circle
function animatedSVG() {
  // Here we can set a condition for ending the animation
  if (false) {
    console.log("Ending animation");
    clearInterval(animationInterval);
    return;
  }

  // If the condition for ending the animation wasn't met, then we'll animate it
  // Get info about how far along animation we are
  {
    // Count up how far along the animation we are
    currentLength += (frameLength / animationLength) * currentDirection;

    // If the animation is at the end, reverse the direction
    if (currentLength >= 1) {
      currentDirection = -1;
      currentLength = 2 - currentLength;
    } else if (currentLength <= 0) {
      // Likewise, if the animation has returned to the start, make it go forward again
      currentDirection = 1;
      currentLength = 0 - currentLength;
    }
  }

  // Get the coordinates of a point on the curve based on how far along the animation we currently are
  var [targetX, targetY] = bezierPoint(startX, controlX, endX, startY, controlY, endY, currentLength);

  // Get the normal vector for this point
  var [normalX, normalY] = bezierNormal(startX, controlX, endX, startY, controlY, endY, currentLength);

  // The normal is the opposite direction of the direction we desire
  normalX *= -1;
  normalY *= -1;

  // Multiply the normal vector by the offset we want
  var offsetX = normalX * (circleRadius + strokeWidth);
  var offsetY = normalY * (circleRadius + strokeWidth);

  // Add the point and offset together to get an offset point
  targetX += offsetX;
  targetY += offsetY;

  // Update the center of the circle to the offset point, and it will look as if it's rolling on the edge of the bezier-curve
  circle.setAttribute("cx", targetX);
  circle.setAttribute("cy", targetY);
}

// Returns a point on a quadratic bezier curve
function pointOnBezier(p0, p1, p2, t) {
  return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p2);
}

// Returns the velocity for one dimension of a quadratic bezier curve
function velocityOfBezier(p0, p1, p2, t) {
  return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
}

// Returns the acceleration for one dimension of a quadratic bezier curve
function accelerationOfBezier(p0, p1, p2, t) {
  return 2 * (p2 - 2 * p1 + p0);
}

// Returns an array containing two dimensional position of a point on a quadratic bezier curve
function bezierPoint(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var pointA = pointOnBezier(p0A, p1A, p2A, t);
  var pointB = pointOnBezier(p0B, p1B, p2B, t);
  return [ pointA, pointB ];
}

// Returns an array containing two dimensional velocity of a point on a quadratic bezier curve
function bezierVelocity(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var velocityA = velocityOfBezier(p0A, p1A, p2A, t);
  var velocityB = velocityOfBezier(p0B, p1B, p2B, t);
  return [ velocityA, velocityB ];
}

// Returns an array containing two dimensional acceleration of a point on a quadratic bezier curve
function bezierAcceleration(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var accelerationA = accelerationOfBezier(p0A, p1A, p2A, t);
  var accelerationB = accelerationOfBezier(p0B, p1B, p2B, t);
  return [ accelerationA, accelerationB ];
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

// Returns a two dimensional cross product (scalar)
function crossProductTwoDimensions(a, b, c, d){
  return a*d - b*c;
}

// Returns a three dimensional cross product
function crossProductThreeDimensions(a1, a2, a3, b1, b2, b3){
  var s1 = a2*b3-a3*b2;
  var s2 = a3*b1-a1*b3;
  var s3 = a1*b2-a2*b1;
  return [ s1, s2, s3 ];
}

// Returns the binormal as a scalar, since this is for a two a two dimensional space
// Functionally not used, as only the unit version of this is to be used
// The unit binomarl for a two dimensional space is just 1 on the 3rd dimension
function bezierBinormal(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var [ velocityA, velocityB ] = bezierVelocity(p0A, p1A, p2A, p0B, p1B, p2B, t);
  var [ accelerationA, accelerationB ] = bezierAcceleration(p0A, p1A, p2A, p0B, p1B, p2B, t);
  return crossProductTwoDimensions(velocityA, velocityB, accelerationA, accelerationB);
}

// Returns the normal vector for a two dimensional quadratic bezier curve
function bezierNormal(p0A, p1A, p2A, p0B, p1B, p2B, t){
  var [ tangentA, tangentB ] = bezierTangentVector(p0A, p1A, p2A, p0B, p1B, p2B, t);
  var [ cross1, cross2, cross3 ] = crossProductThreeDimensions(0, 0, 1, tangentA, tangentB, 0);
  return [ cross1, cross2 ];
}

///////////////////////////////////////////////////////////////
// The following are extra functions added to showcase steps //
// And make the process more easily understandable           //
///////////////////////////////////////////////////////////////

// This gets the circle by its id, so that we can change its attributes
const walkthroughCircle1 = document.getElementById("walkthrough-bezier-point");
// Calls the walkthroughBezierPoint function every "frameLength" milliseconds
var walkthroughPointInterval = setInterval(walkthroughBezierPoint, frameLength);
// A function for showcasing an incomplete step of the circle rolling on the curve
function walkthroughBezierPoint(){
  // Get the coordinates of a point on the curve based on how far along the animation we currently are
  var [targetX, targetY] = bezierPoint(startX, controlX, endX, startY, controlY, endY, currentLength);

  // Update the center of the circle to the coordinates
  walkthroughCircle1.setAttribute("cx", targetX);
  walkthroughCircle1.setAttribute("cy", targetY);
}

// This gets the circle by its id, so that we can change its attributes
const walkthroughCircle2 = document.getElementById("walkthrough-normal-vector");
// Calls the walkthroughBezierPoint function every "frameLength" milliseconds
var walkthroughNormalInterval = setInterval(walkthroughNormalVector, frameLength);
// A function for showcasing an incomplete step of the circle rolling on the curve
function walkthroughNormalVector(){
  // Get the coordinates of a point on the curve based on how far along the animation we currently are
  var [targetX, targetY] = bezierPoint(startX, controlX, endX, startY, controlY, endY, currentLength);
  
  // Get the normal vector for this point
  var [normalX, normalY] = bezierNormal(startX, controlX, endX, startY, controlY, endY, currentLength);

  // Add the point and offset together to get an offset point
  targetX += normalX;
  targetY += normalY;

  // Update the center of the circle to the coordinates
  walkthroughCircle2.setAttribute("cx", targetX);
  walkthroughCircle2.setAttribute("cy", targetY);
}

// This gets the circle by its id, so that we can change its attributes
const walkthroughCircle3 = document.getElementById("walkthrough-distanced");
// Calls the walkthroughBezierPoint function every "frameLength" milliseconds
var walkthroughDistancedInterval = setInterval(walkthroughDistanced, frameLength);
// A function for showcasing an incomplete step of the circle rolling on the curve
function walkthroughDistanced(){
  // Get the coordinates of a point on the curve based on how far along the animation we currently are
  var [targetX, targetY] = bezierPoint(startX, controlX, endX, startY, controlY, endY, currentLength);
  
  // Get the normal vector for this point
  var [normalX, normalY] = bezierNormal(startX, controlX, endX, startY, controlY, endY, currentLength);

  // Multiply the normal vector by the length of the offset we want
  var offsetX = normalX * (circleRadius + strokeWidth);
  var offsetY = normalY * (circleRadius + strokeWidth);

  // Add the point and offset together to get an offset point
  targetX += offsetX;
  targetY += offsetY;

  // Update the center of the circle to the target coordinates
  walkthroughCircle3.setAttribute("cx", targetX);
  walkthroughCircle3.setAttribute("cy", targetY);
}

// This gets the circle by its id, so that we can change its attributes
const walkthroughCircleDone = document.getElementById("walkthrough-done");
// Calls the walkthroughBezierPoint function every "frameLength" milliseconds
var walkthroughDoneInterval = setInterval(walkthroughDone, frameLength);
// A function for showcasing an incomplete step of the circle rolling on the curve
function walkthroughDone(){
  // Get the coordinates of a point on the curve based on how far along the animation we currently are
  var [targetX, targetY] = bezierPoint(startX, controlX, endX, startY, controlY, endY, currentLength);
  
  // Get the normal vector for this point
  var [normalX, normalY] = bezierNormal(startX, controlX, endX, startY, controlY, endY, currentLength);

  // Make the normal vectors point the way we want
  normalX *= -1;
  normalY *= -1;

  // Multiply the normal vector by the length of the offset we want
  var offsetX = normalX * (circleRadius + strokeWidth);
  var offsetY = normalY * (circleRadius + strokeWidth);

  // Add the point and offset together to get an offset point
  targetX += offsetX;
  targetY += offsetY;

  // Update the center of the circle to the target coordinates
  walkthroughCircleDone.setAttribute("cx", targetX);
  walkthroughCircleDone.setAttribute("cy", targetY);
}
