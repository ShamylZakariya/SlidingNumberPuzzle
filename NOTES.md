# NOTES

## SOLVER
So, since the solver is (presently) pre-order, we may find a solution, but it's terrible. We need to do some kind of breadth not depth based search. This is going to require some kind of unrolling the recorsive approach and then tacking our options in another way. I'm sure this is a well-solved problem.
http://www.kopf.com.br/kaplof/how-to-solve-any-slide-puzzle-regardless-of-its-size/
https://www.giantbomb.com/forums/sliding-block-puzzles-232130/a-fool-proof-guide-to-solving-every-solvable-slidi-1802039/
https://entertainment.howstuffworks.com/puzzles/sliding-puzzles3.htm

The solution, in the general case, is to move tile 0 to 0 position, 1 to 1 position, 2 to 2 position, and so on. It's purely mechanical. There may be a faster solution, but this one always works.

## TODO
1. Add animations when playing a tile, with a callback for when tile motion is done.