
(function(){
var
	tetris = {
		brickSize:       30,
		brickBorderSize: 2,
		mainWinWidth:    10,
		mainWinHeight:   20,
		levelUpScore:    150,

		level:   1,
		score:   0,
		singles: 0,
		doubles: 0,
		triples: 0,
		quads:   0,

		bricks:       [],
		pile:         [],
		pileAnimLine: [],
		pileAnimDrop: [],
		gameStart:    true,
		gameOver:     false,
		paused:       false,
		keyPressed:   false,
		shapeCount:   0,

		keyDrop:   32, // Space bar
		keyLeft:   37, // Left key
		keyRotate: 38, // Up key
		keyRight:  39, // Right key
		keyDown:   40, // Down key
		keyPause:  19, // Pause key
		keyStop:   27, // Esc key

		init: function()
		{
		  this.gameStart = true;
		  this.keyPressed = false;
		  this.gameOver = false;
		  this.paused = false;
		  this.keyPressed = false;
		  this.shapeCount = 0;
		  this.bricks = [];
		  this.pileAnimDrop = [];
		  this.pileAnimLine = [];
		  this.pile = [];
		  this.level = 1;
		  this.score = 0;
		  this.singles = 0;
		  this.doubles = 0;
		  this.triples = 0;
		  this.quads = 0;
			MUX.tetris.mainWin = document.getElementById('tetris-main');
			MUX.tetris.nextWin = document.getElementById('tetris-next-inner');
			MUX.tetris.message = document.getElementById('tetris-message')

			MUX.tetris.message.innerHTML = '<p>New game <span>Press any key to start</span></p>';

			document.onkeydown = MUX.tetris.keyListener;
		},

		newGame: function()
		{
			for ( var hor = 0; hor < MUX.tetris.mainWinWidth; hor ++ )
			{
				if ( !MUX.tetris.pile[hor] ) MUX.tetris.pile[hor] = [];

				MUX.tetris.pileAnimLine[hor] = [];
				MUX.tetris.pileAnimDrop[hor] = [];

				for ( var ver = 0; ver < MUX.tetris.mainWinHeight; ver ++ )
				{
					if ( MUX.tetris.pile[hor][ver] )
					{
						MUX.tetris.mainWin.removeChild(MUX.tetris.pile[hor][ver]);
					}

					MUX.tetris.pile[hor][ver] = false;

					MUX.tetris.pileAnimLine[hor][ver] = false;
					MUX.tetris.pileAnimDrop[hor][ver] = false;
				}
			}

			MUX.tetris.level   = 1;
			MUX.tetris.score   = 0;
			MUX.tetris.singles = 0;
			MUX.tetris.doubles = 0;
			MUX.tetris.triples = 0;
			MUX.tetris.quads   = 0;

			MUX.tetris.updateScore();

			MUX.tetris.newShape();
		},

		newShape: function()
		{
			MUX.tetris.shapeCount ++;

			MUX.tetris.shapeNum     = typeof(MUX.tetris.shapeNumNext) != 'undefined' ? MUX.tetris.shapeNumNext : Math.floor(Math.random() * 6);
			MUX.tetris.shapeNumNext = Math.floor(Math.random() * 7);
			MUX.tetris.shapeRot     = typeof(MUX.tetris.shapeRotNext) != 'undefined' ? MUX.tetris.shapeRotNext : Math.floor(Math.random() * 4);
			MUX.tetris.shapeRotNext = Math.floor(Math.random() * 4);
			MUX.tetris.shapePosHor  = Math.floor(Math.random() * ( MUX.tetris.mainWinWidth - 6 )) + 3;
			MUX.tetris.shapePosVer  = -1;

			MUX.tetris.drawShape();

			MUX.tetris.drawNext();

			MUX.tetris.shapeLanded = false;

			clearInterval(MUX.tetris.intval);

			MUX.tetris.intval = setInterval('MUX.tetris.timeStep()', 2000 / MUX.tetris.level);
		},

		newBrick: function(color, colorLight, colorDark)
		{
			var
				brick = document.createElement('div')
				;

			brick.setAttribute('style', 'background: ' + color + '; border-color: ' + colorLight + ' ' + colorDark + ' ' + colorDark + ' ' + colorLight + '; border-width: ' + MUX.tetris.brickBorderSize + 'px; border-style: solid; height: ' + ( MUX.tetris.brickSize - MUX.tetris.brickBorderSize * 2 ) + 'px; left: 0; top: 0; width: ' + ( MUX.tetris.brickSize - MUX.tetris.brickBorderSize * 2 ) + 'px; position: absolute;');

			return brick;
		},

		drawShape: function()
		{
			var
				brickCount = 0,
				move       = true
				;

			MUX.tetris.brickPos = [];

			for ( var ver = 0; ver < 4; ver ++ )
			{
				for ( var hor = 0; hor < 4; hor ++ )
				{
					if ( MUX.tetris.brickLib[MUX.tetris.shapeNum][ver * 4 + hor + MUX.tetris.shapeRot * 16] )
					{
						MUX.tetris.brickPos[brickCount] = {
							hor: hor + MUX.tetris.shapePosHor,
							ver: ver + MUX.tetris.shapePosVer
							}

						if ( MUX.tetris.collision(MUX.tetris.brickPos[brickCount].hor, MUX.tetris.brickPos[brickCount].ver) ) move = false;

						brickCount ++;
					}
				}
			}

			if ( move && !MUX.tetris.paused && !MUX.tetris.gameOver )
			{
				var prevBricks = MUX.tetris.bricks ? MUX.tetris.bricks.slice(0) : false;

				for ( var i = 0; i < brickCount; i ++ )
				{
					MUX.tetris.bricks[i] = MUX.tetris.newBrick(
						MUX.tetris.brickLib[MUX.tetris.shapeNum][64], MUX.tetris.brickLib[MUX.tetris.shapeNum][65], MUX.tetris.brickLib[MUX.tetris.shapeNum][66]
						);

					MUX.tetris.bricks[i].num = MUX.tetris.shapeCount;

					MUX.tetris.bricks[i].style.left = MUX.tetris.brickPos[i].hor * MUX.tetris.brickSize + 'px';
					MUX.tetris.bricks[i].style.top  = MUX.tetris.brickPos[i].ver * MUX.tetris.brickSize + 'px';
				}

				for ( var i = 0; i < brickCount; i ++ ) // Using seperate for-loops to reduce flickering
				{
					// Draw brick in new position
					MUX.tetris.mainWin.appendChild(MUX.tetris.bricks[i]);
				}

				for ( var i = 0; i < brickCount; i ++ )
				{
					// Remove brick in prev position
					if ( prevBricks[i] && prevBricks[i].num == MUX.tetris.shapeCount )
					{
						MUX.tetris.mainWin.removeChild(prevBricks[i]);
					}
				}

				MUX.tetris.prevShapeRot    = MUX.tetris.shapeRot;
				MUX.tetris.prevShapePosHor = MUX.tetris.shapePosHor;
				MUX.tetris.prevShapePosVer = MUX.tetris.shapePosVer;
				MUX.tetris.prevBrickPos    = MUX.tetris.brickPos.slice(0);
			}
			else
			{
				// Collision detected, keep shape in previous position
				MUX.tetris.shapeRot    = MUX.tetris.prevShapeRot;
				MUX.tetris.shapePosHor = MUX.tetris.prevShapePosHor;
				MUX.tetris.shapePosVer = MUX.tetris.prevShapePosVer;
				MUX.tetris.brickPos    = MUX.tetris.prevBrickPos.slice(0);
			}
		},

		drawNext: function()
		{
			MUX.tetris.nextWin.innerHTML = '';

			for ( var ver = 0; ver < 4; ver ++ )
			{
				for ( var hor = 0; hor < 4; hor ++ )
				{
					if ( MUX.tetris.brickLib[MUX.tetris.shapeNumNext][ver * 4 + hor + MUX.tetris.shapeRotNext * 16] )
					{
						brick = MUX.tetris.newBrick(
							MUX.tetris.brickLib[MUX.tetris.shapeNumNext][64], MUX.tetris.brickLib[MUX.tetris.shapeNumNext][65], MUX.tetris.brickLib[MUX.tetris.shapeNumNext][66]
							);

						brick.style.left = hor * MUX.tetris.brickSize + 'px';
						brick.style.top  = ver * MUX.tetris.brickSize + 'px';

						MUX.tetris.nextWin.appendChild(brick);
					}
				}
			}
		},

		collision: function(hor, ver)
		{
			// Left wall
			if ( hor < 0 )
			{
				if ( MUX.tetris.keyPressed == MUX.tetris.keyRotate )
				{
					// No room to rotate, try moving right
					if ( !MUX.tetris.collision(hor + 1, ver) )
					{
						MUX.tetris.shapePosHor ++;

						MUX.tetris.drawShape();

						return true;
					}
					else
					{
						MUX.tetris.shapeRot --;

						return true;
					}
				}

				return true;
			}

			// Right wall
			if ( hor >= MUX.tetris.mainWinWidth )
			{
				if ( MUX.tetris.keyPressed == MUX.tetris.keyRotate )
				{
					// No room to rotate, try moving left
					if ( !MUX.tetris.collision(hor - 1, ver) )
					{
						MUX.tetris.shapePosHor --;

						MUX.tetris.drawShape();

						return true;
					}
					else
					{
						MUX.tetris.shapeRot --;

						return true;
					}
				}

				return true;
			}

			// Floor
			if ( ver >= MUX.tetris.mainWinHeight )
			{
				if ( MUX.tetris.keyPressed != MUX.tetris.keyRotate ) MUX.tetris.shapePosVer --;

				MUX.tetris.shapeLanded = true;

				return true;
			}

			// Pile
			if ( MUX.tetris.pile[hor][ver] )
			{
				if ( MUX.tetris.shapePosVer > MUX.tetris.prevShapePosVer ) MUX.tetris.shapeLanded = true;

				return true;
			}

			return false;
		},

		timeStep: function()
		{
		  if(MUX.$('tetris-wrap') == null) {
		    return;
		  }
			MUX.tetris.shapePosVer ++;

			MUX.tetris.drawShape();

			if ( MUX.tetris.shapeLanded )
			{
				for ( var i in MUX.tetris.bricks )
				{
					MUX.tetris.pile[MUX.tetris.brickPos[i].hor][MUX.tetris.brickPos[i].ver] = MUX.tetris.bricks[i];
				}

				// Check for completed lines
				var lines = 0;

				for ( var ver = 0; ver < MUX.tetris.mainWinHeight; ver ++ )
				{
					var line = true;

					for ( var hor = 0; hor < MUX.tetris.mainWinWidth; hor ++ )
					{
						if ( !MUX.tetris.pile[hor][ver] ) line = false;
					}

					if ( line )
					{
						lines ++;

						// Remove line
						for ( var hor = 0; hor < MUX.tetris.mainWinWidth; hor ++ )
						{
							if ( MUX.tetris.pile[hor][ver] )
							{
								MUX.tetris.pileAnimLine[hor][ver] = MUX.tetris.pile[hor][ver];

								setTimeout('MUX.tetris.mainWin.removeChild(MUX.tetris.pileAnimLine[' + hor + '][' + ver + ']);', hor * 50);

								MUX.tetris.pile[hor][ver] = false;
							}
						}

						// Drop lines
						for ( var hor = 0; hor < MUX.tetris.mainWinWidth; hor ++ )
						{
							for ( var ver2 = ver; ver2 > 0; ver2 -- ) // From bottom to top
							{
								if ( MUX.tetris.pile[hor][ver2] )
								{
									MUX.tetris.pileAnimDrop[hor][ver2] = MUX.tetris.pile[hor][ver2];

									setTimeout('MUX.tetris.pileAnimDrop[' + hor + '][' + ver2 + '].style.top = ( ' + ver2 + ' + 1 ) * MUX.tetris.brickSize + \'px\';', MUX.tetris.mainWinWidth * 50);

									MUX.tetris.pile[hor][ver2 + 1] = MUX.tetris.pile[hor][ver2];
									MUX.tetris.pile[hor][ver2]     = false;
								}
							}
						}
					}
				}

				MUX.tetris.updateScore(lines);

				// Check for game over
				for ( var hor = 0; hor < MUX.tetris.mainWinWidth; hor ++ )
				{
					if ( MUX.tetris.pile[hor][0] )
					{
						MUX.tetris.doGameOver();

						return;
					}
				}

				MUX.tetris.newShape();
			}
		},

		updateScore: function(lines)
		{
			var oldScore = MUX.tetris.score;

			if ( lines )
			{
				MUX.tetris.score += lines * lines + lines * 10;
			}

			for ( i = oldScore; i < MUX.tetris.score; i ++ )
			{
				setTimeout('document.getElementById(\'tetris-score\').innerHTML = \'' + i + '\';', ( i - oldScore ) * 20);
			}

			MUX.tetris.level = Math.floor(MUX.tetris.score / MUX.tetris.levelUpScore) + 1;

			document.getElementById('tetris-level').innerHTML = MUX.tetris.level;

			if ( lines == 1 )
			{
				MUX.tetris.singles ++;

				document.getElementById('tetris-singles').innerHTML = MUX.tetris.singles;
			}

			if ( lines == 2 )
			{
				MUX.tetris.flashMessage('<p class="tetris-double">Double!</p>');

				MUX.tetris.doubles ++;

				document.getElementById('tetris-doubles').innerHTML = MUX.tetris.doubles;
			}

			if ( lines == 3 )
			{
				MUX.tetris.flashMessage('<p class="tetris-double">Triple!</p>');

				MUX.tetris.triples ++;

				document.getElementById('tetris-triples').innerHTML = MUX.tetris.triples;
			}

			if ( lines == 4 )
			{
				MUX.tetris.flashMessage('<p class="tetris-double">Tetris!</p>');

				MUX.tetris.quads ++;

				document.getElementById('tetris-quads').innerHTML = MUX.tetris.quads;
			}
		},

		flashMessage: function(message)
		{
			MUX.tetris.message.innerHTML = message;

			setTimeout('MUX.tetris.message.innerHTML = \'\';', 1000);
		},

		doGameOver: function()
		{
			clearInterval(MUX.tetris.intval);

			MUX.tetris.message.innerHTML = '<p>Game over <span>Press Spacebar to continue</span</p>';

			MUX.tetris.gameOver = true;
		},

		keyListener: function(e)
		{
			if( !e ) // IE
			{
				e = window.event;
			}

			MUX.tetris.keyPressed = e.keyCode;

			if ( MUX.tetris.gameStart )
			{
				MUX.tetris.gameStart = false;

				MUX.tetris.message.innerHTML = '';

				MUX.tetris.newGame();
			}
			else
			{
				if ( MUX.tetris.gameOver && e.keyCode == MUX.tetris.keyDrop )
				{
					MUX.tetris.gameOver = false;

					MUX.tetris.message.innerHTML = '';

					MUX.tetris.newGame();
				}
				else if ( !MUX.tetris.gameOver )
				{
					if ( e.keyCode == MUX.tetris.keyStop || e.keyCode == MUX.tetris.keyPause )
					{
						MUX.tetris.paused = !MUX.tetris.paused;

						if ( MUX.tetris.paused )
						{
							MUX.tetris.message.innerHTML = '<p>Paused <span>Press Esc to resume</span</p>';
						}
						else
						{
							MUX.tetris.message.innerHTML = '';
						}

						return false;
					}

					if ( !MUX.tetris.paused )
					{
						if ( e.keyCode == MUX.tetris.keyDrop )
						{
							clearInterval(MUX.tetris.intval);

							MUX.tetris.intval = setInterval('MUX.tetris.timeStep()', 20);

							return false;
						}

						if ( e.keyCode == MUX.tetris.keyLeft )
						{
							MUX.tetris.shapePosHor --;

							MUX.tetris.drawShape();

							return false;
						}

						if ( e.keyCode == MUX.tetris.keyRotate )
						{
							MUX.tetris.shapeRot = ( MUX.tetris.shapeRot + 1 ) % 4;

							MUX.tetris.drawShape();

							return false;
						}

						if( e.keyCode == MUX.tetris.keyRight )
						{
							MUX.tetris.shapePosHor ++;

							MUX.tetris.drawShape();

							return false;
						}

						if ( e.keyCode == MUX.tetris.keyDown )
						{
							MUX.tetris.shapePosVer ++;

							MUX.tetris.drawShape();

							return false;
						}
					}
				}
			}

			return true;
		},

		brickLib: {
			0: [
				1, 0, 0, 0,
				1, 0, 0, 0,
				1, 1, 0, 0,
				0, 0, 0, 0,

				1, 1, 1, 0,
				1, 0, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				0, 1, 1, 0,
				0, 0, 1, 0,
				0, 0, 1, 0,
				0, 0, 0, 0,

				0, 0, 0, 0,
				0, 0, 1, 0,
				1, 1, 1, 0,
				0, 0, 0, 0,

				'#F90', '#FC0', '#F60'
				],
			1: [
				0, 1, 0, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,

				0, 0, 0, 0,
				1, 1, 1, 1,
				0, 0, 0, 0,
				0, 0, 0, 0,

				0, 1, 0, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,

				0, 0, 0, 0,
				1, 1, 1, 1,
				0, 0, 0, 0,
				0, 0, 0, 0,

				'#C00', '#E00', '#B00'
				],

			2: [
				1, 1, 0, 0,
				1, 0, 0, 0,
				1, 0, 0, 0,
				0, 0, 0, 0,

				1, 1, 1, 0,
				0, 0, 1, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				0, 0, 1, 0,
				0, 0, 1, 0,
				0, 1, 1, 0,
				0, 0, 0, 0,

				0, 0, 0, 0,
				1, 0, 0, 0,
				1, 1, 1, 0,
				0, 0, 0, 0,

				'#0C0', '#0E0', '#0A0'
				],

			3: [
				1, 0, 0, 0,
				1, 1, 0, 0,
				1, 0, 0, 0,
				0, 0, 0, 0,

				1, 1, 1, 0,
				0, 1, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				0, 0, 1, 0,
				0, 1, 1, 0,
				0, 0, 1, 0,
				0, 0, 0, 0,

				0, 0, 0, 0,
				0, 1, 0, 0,
				1, 1, 1, 0,
				0, 0, 0, 0,

				'#00C', '#00E', '#00A'
				],

			4: [
				1, 1, 0, 0,
				1, 1, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				1, 1, 0, 0,
				1, 1, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				1, 1, 0, 0,
				1, 1, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				1, 1, 0, 0,
				1, 1, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				'#60C', '#80E', '#40A'
				],

			5: [
				0, 1, 1, 0,
				1, 1, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				1, 0, 0, 0,
				1, 1, 0, 0,
				0, 1, 0, 0,
				0, 0, 0, 0,

				0, 1, 1, 0,
				1, 1, 0, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				1, 0, 0, 0,
				1, 1, 0, 0,
				0, 1, 0, 0,
				0, 0, 0, 0,

				'#CCC', '#EEE', '#AAA'
				],

			6: [
				1, 1, 0, 0,
				0, 1, 1, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				0, 1, 0, 0,
				1, 1, 0, 0,
				1, 0, 0, 0,
				0, 0, 0, 0,

				1, 1, 0, 0,
				0, 1, 1, 0,
				0, 0, 0, 0,
				0, 0, 0, 0,

				0, 1, 0, 0,
				1, 1, 0, 0,
				1, 0, 0, 0,
				0, 0, 0, 0,

				'#CC0', '#EE0', '#AA0'
				]
		}
	};
	MUX.tetris = tetris;
})();