using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.U2D;
using System.Threading.Tasks;

public delegate void CallMethodOn();

public class Phase{
    public string phaseName;
    public Action callMethodOn;
    public Phase(string _phaseName,Action methodToCallOnInit = null){
        callMethodOn = methodToCallOnInit;
        phaseName = _phaseName;
    }
}

public class Game : MonoBehaviour
{   

    [Header("TEST INPUT")]    
    public List<int> testDiceValue;
    public bool isTestEnabled;
    public float popUpMessageTime;
    public float hideUpMessageTime;
    public float hideTime; //for counters 
    public float showTime; //for counters
    public float counterScaleTo;

    [Space]

    public float timeScaler;
    public float timeScaler2;

    [Header("References")]
    public Building buildingReference;
    public Dice3D diceReference;
    public Depth currentDepthReference;
    public Phase currentPhaseReference;

    [Space]

    [Header("Collections")]
    public List<Dice3D> constDiceList;
    public List<Dice3D> dynamicDiceList;
    public List<Dice3D> diceListToReroll;
    public List<Dice3D> diceListToBuy;
    public List<Depth> depthList;
    public List<Phase> phaseList;
    public List<Phase> calculatedPhaseList;
    public List<Action> phasesCallbackList;
    public List<Building> shopBuildingsList;
    private List<Building> uniqueBuldingsList;

    public List<Text> shopCaptionForLocalisation;
    public List<Text> descriptionForLocalisation;

    public List<Cell> minersList; 
    public List<Cell> mushroomFarmsCellsList;
    public List<Cell> mineCellList;

    public List<Sprite> phaseImageList;

    [Header("UI")]
    public RectTransform loadSceneRect;
    public Button mainButton;
    public Text mainButtonText;

    public Text goldText;
    public Text pointsText;
    public Text roundText;

    public CanvasGroup endGamePanel;
    public Text endGamePointsText;

    public SpriteAtlas shopButtonAtlas;
    public Image currentPhaseSprite;

    public Text freeRerollCountText;

    [Header("Counters")]
    public int countOfFreeReroll;
    public int currentCountOfFreeReroll;
    
    public int buildCount;
    public int digCount;

    public int points;
    public int goldCount;
    public int roundCounter;

    public int countOfArenas;
    public int countOfBrew;
    public int countOfMushroomFarm;
    public int countOfBlacksmith;
    public int countOfCarpenter;
    public int countOfStatues;

    private int countOfThree;
    private int countOfSix;

    [Header("Localisations")]
    public List<LanguagePack> languagesList;
    public LanguagePack currentLanguage;

    [Header("Other")]
    public WarningSystem messageToUserSystem;

    private void InitShopButtons(){
        foreach(Building b in shopBuildingsList){
            b.button.GetComponent<Image>().sprite = shopButtonAtlas.GetSprite(b.props.id.ToString());
        }
    }
    //AWAKE
    public void Awake(){
        Application.targetFrameRate = 60;
        InitUILocalisation();
        //loadSceneRect.DOAnchorMin(new Vector2(0,1),.7f).SetEase(Ease.Linear).OnComplete(()=>{initGame();});
        initGame();
    }

    public void initGame(){
        uniqueBuldingsList = new List<Building>();
        countOfStatues = 0;
        freeRerollCountText.text = "";
        InitShopButtons();
        if(Application.systemLanguage==SystemLanguage.English){
            currentLanguage = languagesList[0];
        }
        if(Application.systemLanguage==SystemLanguage.Russian){
            currentLanguage = languagesList[1];
            
        }
        for(int i = 0;i < currentLanguage.shopBuildingLocalisationData.Count;i++){
            shopCaptionForLocalisation[i].text = currentLanguage.shopBuildingLocalisationData[i].caption;
            descriptionForLocalisation[i].text = currentLanguage.shopBuildingLocalisationData[i].description;
        }

        buildCount = 0;
        countOfThree = 0;
        currentDepthReference = depthList[0];
        currentCountOfFreeReroll = 0;
        countOfFreeReroll = 0;
        diceReference = null;

        updateUiText(goldText,"0");
        updateUiText(roundText,"1");
        updateUiText(pointsText,"0");

        phaseList = new List<Phase>(){
            new Phase(currentLanguage.getPhaseNameByKey("ROUNDSTART"),()=>{initStartRound();}),
            new Phase(currentLanguage.getPhaseNameByKey("THROWPHASE"),()=>{initRollPhase();}),
            new Phase(currentLanguage.getPhaseNameByKey("REROLLPHASE"),()=>{goToRerollPhase();}),
            new Phase(currentLanguage.getPhaseNameByKey("BUYVALUESPHASE"),()=>{initBuyValuesPhase();}),
            new Phase(currentLanguage.getPhaseNameByKey("FIGHTERPHASE"),()=>{initFighterPhase();}),
            new Phase(currentLanguage.getPhaseNameByKey("DRINKERSPHASE"),()=>{initDrinkerPhase();}),
            new Phase(currentLanguage.getPhaseNameByKey("COOKINGPHASE"),()=>{initFarmerPhase();}),
            new Phase(currentLanguage.getPhaseNameByKey("DIGGINGPHASE"),()=>{initDiggerPhase();}),
            new Phase(currentLanguage.getPhaseNameByKey("BUILDERSPHASE"),()=>{initBuilderPhase();}),
            new Phase(currentLanguage.getPhaseNameByKey("MINERSPHASE"),()=>{initMinersPhase();}),
            new Phase(currentLanguage.getPhaseNameByKey("ENDROUNDPHASE"),()=>{checkEndGame();})
        };
        calculatedPhaseList = new List<Phase>(){
            phaseList[0],
            phaseList[1],
            phaseList[2],
            phaseList[3]
        };

        phasesCallbackList = new List<Action>(){
            clearStartRound,
            clearRollPhase,
            clearRerollPhase,
            clearBuyValuePhase,
            clearResolveOnes,
            clearResolveTwo,
            clearResolveThree,
            clearResolveFourth,
            clearResolveFives,
            clearResolveSix,
            clearCheckEndGamePhase
        };
        currentPhaseReference = calculatedPhaseList[0];
        currentPhaseReference.callMethodOn();
    }

    //test mode
    public void setTestList(List<int> sequenceToSet){
        mainButton.onClick.RemoveAllListeners();
        for(int i = 0;i< dynamicDiceList.Count;i++){
            if (i == dynamicDiceList.Count-1){
                dynamicDiceList[i].rollDieTest(sequenceToSet[i],()=>{goToRerollPhase();});
            }
            else{
                dynamicDiceList[i].rollDieTest(sequenceToSet[i]);
            }
        } 
    }

    //global
    public void goToNextPhase(Phase destinationPhase = null){
        
        phasesCallbackList[phaseList.IndexOf(currentPhaseReference)]();

        if(destinationPhase != null){
            currentPhaseReference = destinationPhase;
            if(currentPhaseReference.callMethodOn != null){
                currentPhaseReference.callMethodOn();
            }
            if(currentPhaseReference == null){
                return;
            }
            return;
        }

        if(currentPhaseReference == calculatedPhaseList[^1]){
            currentPhaseReference = calculatedPhaseList[0];
        }
        else{
            currentPhaseReference = calculatedPhaseList[calculatedPhaseList.IndexOf(currentPhaseReference)+1];
        }

        messageToUserSystem.showRoundPhaseMessage(currentPhaseReference.phaseName,popUpMessageTime,hideUpMessageTime,()=>{
            if(currentPhaseReference.callMethodOn != null){
                currentPhaseReference.callMethodOn();
            }
        });
    }

    public int getDiceCountByValue(int value){
        int counter = 0;
        foreach(Dice3D d in dynamicDiceList){
            if(d.oldValue == value){
                counter++;
            }
        }
        return counter;
    }

    private void InitUILocalisation(){
    }

    public void updateUiText(Text txt,string newValue){ //перенести потом в warning system
        txt.text = newValue;
    }

    public void reassignMainButtonNewAction(string newButtonCaption,Color newColor,Action newAction){
        
        mainButton.onClick.RemoveAllListeners();
        
        if(newAction == null){
            mainButton.onClick.RemoveAllListeners();
        }
        else{
            mainButton.onClick.AddListener(()=>{newAction();});
        }
        mainButton.GetComponent<Image>().color = newColor;
        mainButtonText.text = newButtonCaption;
    }

    //0) Фаза начала раунда
    public void initStartRound(){
        currentCountOfFreeReroll = countOfFreeReroll;
        dynamicDiceList.Clear();
        
        timeScaler = 0.45f;
        timeScaler2 = 0.45f;
        
        countOfThree = 0;

        foreach(Dice3D d in constDiceList){
            d.markedForBuy = false;
            d.valueToBuy = -1;
            d.oldValue = -1;
            d.reset3DDiceColor();
            dynamicDiceList.Add(d);
        }
        goToNextPhase();
    }
    public void clearStartRound(){}


    //1) фаза броска
    public void initRollPhase(){
        currentPhaseSprite.sprite = phaseImageList[0];
        reassignMainButtonNewAction(currentLanguage.getValueForButton("THROW"),Color.green,rollForInitValues);
    }

    public void rollForInitValues(){
        if(isTestEnabled){
            mainButton.onClick.RemoveAllListeners();
            setTestList(testDiceValue);
        }
        else{
            mainButton.onClick.RemoveAllListeners();
            for(int i = 0;i< dynamicDiceList.Count;i++){
                if (i == dynamicDiceList.Count-1){
                    dynamicDiceList[i].roll3DDieAsync(timeScaler,()=>{goToNextPhase();});
                }
                else{
                    dynamicDiceList[i].roll3DDieAsync(timeScaler);
                }
                timeScaler += timeScaler2;
            }  
        }
    }

    public void clearRollPhase(){reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,null);}
    //2) фаза переброса значений

    public void markForReroll(Dice3D d){
        if(diceListToReroll.Contains(d)){
            diceListToReroll.Remove(d);
            d.reset3DDiceColor();
        }
        else if(!diceListToReroll.Contains(d)){
            diceListToReroll.Add(d);
            d.setEmissionColor(new Color(1,0,0,1));
        }
        if(diceListToReroll.Count > 0 ){
            reassignMainButtonNewAction(currentLanguage.getValueForButton("REROLL"),Color.red,reroll);
        }
        else if(diceListToReroll.Count <= 0 ){
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
    }

    public void goToRerollPhase(){
        freeRerollCountText.text = countOfFreeReroll.ToString();
        currentPhaseSprite.sprite = phaseImageList[1];
        reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        foreach(Dice3D d in dynamicDiceList){
            d.onClick2 = ()=>{markForReroll(d);};
        }
    }

    public void reroll(){
        if(diceListToReroll.Count == 0){
            return;
        }
        else{
            mainButton.onClick.RemoveAllListeners();
            foreach(Dice3D d in dynamicDiceList){
                d.onClick = null;
                d.onClick2 = null;
            }
            
            foreach(Dice3D d in diceListToReroll){
                float i = 1;
                if(d != diceListToReroll[^1]){
                    d.roll3DDieAsync(i);
                }
                else{
                    d.roll3DDieAsync(i,()=>{
                        checkForFreeReroll();
                        });
                    return;
                }
                i++;
            }
        }
    }
    
    public void checkForFreeReroll(){
        diceListToReroll.Clear();
        foreach(Dice3D d in dynamicDiceList){
            d.reset3DDiceColor();
        }
        if(currentCountOfFreeReroll==0){
            
            foreach(var dice in dynamicDiceList){
                dice.onClick2 = ()=>{markToDelete(dice);};
            }
            reassignMainButtonNewAction(currentLanguage.getValueForButton("DELETE"),Color.green,deleteAccept);
        }
        else{
            currentCountOfFreeReroll--;
            freeRerollCountText.text = currentCountOfFreeReroll.ToString();
            foreach(Dice3D d in dynamicDiceList){
                d.onClick = null;
                d.onClick2 = null;
                d.onClick2 = ()=>{markForReroll(d);};
            }
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
    }

    public void markToDelete(Dice3D d){
        if(diceReference==null){
            diceReference = d;
            diceReference.setEmissionColor(new Color(1,0,0,1));
            return;
        }
        else{
            diceReference.reset3DDiceColor();
            if(diceReference == d){
                diceReference = null;
            }
            else if(diceReference != d){
                diceReference = d;
                diceReference.setEmissionColor(new Color(1,0,0,1));
            }
        }
    }

    public void deleteAccept(){
        if(diceReference==null){
            return;
        }
        diceReference.onClick = null;
        diceReference.onClick2 = null;
        dynamicDiceList.Remove(diceReference);
        diceReference = null;
        if(dynamicDiceList.Count == 0){
            //go to end round
            goToNextPhase(phaseList[^1]);
        }
        else{
            foreach(Dice3D d in dynamicDiceList){
                d.onClick = null;
                d.onClick2 = null;
                d.onClick = ()=>markForReroll(d);
            }
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
    }

    public void clearRerollPhase(){
        reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,null);
        foreach(Dice3D d in constDiceList){
            d.onClick = null;
            d.onClick2 = null;
        }
        freeRerollCountText.text = "";
    }
    //3) Фаза покупки значений
    public void initBuyValuesPhase(){
        currentPhaseSprite.sprite = phaseImageList[2];
        reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        foreach(Dice3D d in dynamicDiceList){
            d.onClick = null;
            d.onClick = ()=>{selectNewValue(d);};
        }
    }
    public void сonfirmPurchaseOfNewValues(){
        Debug.Log(goldCount.ToString());
        if(goldCount >= diceListToBuy.Count *5){
            goldCount -= diceListToBuy.Count *5;
            messageToUserSystem.updateCounterWithAnimation(goldText,()=>{updateUiText(goldText,goldCount.ToString());});
            foreach(Dice3D d in diceListToBuy){
                d.oldValue = d.valueToBuy;
                d.reset3DDiceColor();
            }
            foreach(Dice3D d in dynamicDiceList){
                d.onClick = null;
                d.onClick = ()=>{selectNewValue(d);};
            }   
            diceListToBuy.Clear();
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
        else{
            return;
        }
    }
    public void selectNewValue(Dice3D d){
        d.valueToBuy++;
        if(d.valueToBuy>6){
            d.valueToBuy = 1;
        }

        d.set3DDiceValue(d.valueToBuy);

        if(d.valueToBuy != d.oldValue){
            d.setEmissionColor(new Vector4(1,1,0,1));
            if(!diceListToBuy.Contains(d)){
                diceListToBuy.Add(d);
            }
        }
        if(d.valueToBuy == d.oldValue){
            d.reset3DDiceColor();
            diceListToBuy.Remove(d);
        }

        if(diceListToBuy.Count > 0){
            reassignMainButtonNewAction(currentLanguage.getValueForButton("ACCEPTBUY"),Color.yellow,сonfirmPurchaseOfNewValues);
        }
        if(diceListToBuy.Count == 0){
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
    }

    public void appointNextPhases(bool onesFlag){
        bool isIncludeOnes = false;
        if(onesFlag){
            while(calculatedPhaseList.Count!=5){
                calculatedPhaseList.RemoveAt(calculatedPhaseList.Count-1);
            }
            isIncludeOnes = false;
        }
        else{
            while(calculatedPhaseList.Count!=4){
                calculatedPhaseList.RemoveAt(calculatedPhaseList.Count-1);
            }
            isIncludeOnes = true;
        }
        if(isIncludeOnes){
            if(getDiceCountByValue(1) > 0){
                calculatedPhaseList.Add(phaseList[4]);
            }
        }
        if(getDiceCountByValue(2) > 0){
            calculatedPhaseList.Add(phaseList[5]);
        }
        if(getDiceCountByValue(3) > 0){

            calculatedPhaseList.Add(phaseList[6]);
            
            if(getDiceCountByValue(4)>=1){
                calculatedPhaseList.Add(phaseList[7]);
            }

            int fiveCount = getDiceCountByValue(5);
            /*if(fiveCount > 1 || (fiveCount>=1 && countOfCarpenter >=1 && getFreeCellsCountToBuild(false)>0) ){
                calculatedPhaseList.Add(phaseList[8]);
            }*/
            if( (fiveCount > 1 && getFreeCellsCountToBuild(false)>0) || (fiveCount>=1 && countOfCarpenter >=1 && getFreeCellsCountToBuild(false)>0) ){
                calculatedPhaseList.Add(phaseList[8]);
            }
            if(getDiceCountByValue(6) > 0 && mineCellList.Count > 0){
                calculatedPhaseList.Add(phaseList[9]);
            }
        }
        else{
            if(mushroomFarmsCellsList.Count > 0){ //если имеем хотя-бы одну ферму
                //обработка четвёрок
                if(currentDepthReference.depthContainBuildings("MushroomFarm") && getDiceCountByValue(4)>=1){ 
                    calculatedPhaseList.Add(phaseList[7]);
                }
                //обработка пятёрок
                int fiveCount = getDiceCountByValue(5);
                float localCounter = 0;
                foreach(Cell mushroomCell in mushroomFarmsCellsList){
                    localCounter += mushroomCell.parentDepth.getDepthsFreeCellsCount();
                }
                if(localCounter != 0 && ((fiveCount>=1 && countOfCarpenter >=1)||fiveCount>1)){
                    calculatedPhaseList.Add(phaseList[8]);       
                }
                //обработка шестёрок
                localCounter = 0;
                foreach(Cell mushroomCell in mineCellList){
                    if(mushroomCell.parentDepth.depthContainBuildings("MushroomFarm")){
                        localCounter++;
                    }
                }
                if(localCounter != 0 && getDiceCountByValue(6) > 0 && mineCellList.Count > 0){
                    calculatedPhaseList.Add(phaseList[9]);
                }
            }
        }
        calculatedPhaseList.Add(phaseList[10]);
    }
    public void clearBuyValuePhase(){
        foreach(Dice3D d in dynamicDiceList){
            d.onClick = null;
        }
        //вот здесь начинаем считать всю хуйню, какие фазы должны быть
        reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,null);
        appointNextPhases(false);
    }
    //4) Фаза разрещения единичек
    public void initFighterPhase(){
        int countOfOnes = getDiceCountByValue(1);
        currentPhaseSprite.sprite = phaseImageList[3];
        if(countOfOnes == 0){
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
            return;
        }
        if(countOfOnes % 2 == 0){
            messageToUserSystem.updateCounterWithAnimation(pointsText,()=>{
                points += getFightersPoints(countOfOnes);
                updateUiText(pointsText,points.ToString());
                reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
            });
        }
        else if(countOfOnes % 2 == 1){
            points += getFightersPoints(countOfOnes-1);
            updateUiText(pointsText,points.ToString());
            foreach(Dice3D d in dynamicDiceList){
                if(d.oldValue == 1 && diceReference==null){
                    diceReference = d;
                    //diceReference.buttonImage.color = Color.gray;
                    diceReference.onClick = null;
                    diceReference = null;
                }
                else{
                    d.onClick = ()=>deleteByFighters(d);   
                }
            }
            reassignMainButtonNewAction(currentLanguage.getValueForButton("DELETE"),Color.red,confirmDeleteFromFighters);
        }
    }

    public void deleteByFighters(Dice3D d){
        if(diceReference==null){
            diceReference = d;
            diceReference.setEmissionColor(new Vector4(1,0,0,1));
        }
        else{
            diceReference.reset3DDiceColor();
            if(diceReference == d){
                diceReference = null;
            }
            else if(diceReference != d){ ///вот этот момент нихера не понял
                diceReference = d;
                diceReference.setEmissionColor(new Vector4(1,0,0,1));
            }
        }
    }

    public void confirmDeleteFromFighters(){
        if(diceReference==null){
            return;
        }
        else{
            foreach(Dice3D d in dynamicDiceList){
                d.onClick = null;
            }
            dynamicDiceList.Remove(diceReference);
            diceReference=null;
        }
        appointNextPhases(true);
        reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
    }

    public int getFightersPoints(int fightersCount){
        if(fightersCount <= 1){
            return 0;
        }
        int pairCount = fightersCount / 2;

        if(countOfArenas == 0){
            return pairCount * 3;
        }
        else if(countOfArenas > 0){
            if(pairCount > countOfArenas){
                return (countOfArenas * 6) + (pairCount-countOfArenas)*3;
            }
            else if(pairCount <= countOfArenas){
                return pairCount * 6;
            }
        }
        return 0;
    }
    public void clearResolveOnes(){}
    
    //5) Фаза разрешения двоек

    public void initDrinkerPhase(){
        currentPhaseSprite.sprite = phaseImageList[4];
        int countOfDrinkers = getDiceCountByValue(2);
        if(countOfDrinkers == 0){
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
        else{
            messageToUserSystem.updateCounterWithAnimation(pointsText,()=>{
                    if(countOfBrew < 1){
                        points += countOfDrinkers;
                    }
                    else{
                        if(countOfDrinkers > countOfBrew){
                            points += (countOfBrew * 2) + (countOfDrinkers-countOfBrew);
                        }
                        else if(countOfDrinkers <= countOfBrew){
                            points += countOfDrinkers * 2;
                        }
                    }
                    updateUiText(pointsText,points.ToString());
                    reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
                });
        }
        
    }
    public void clearResolveTwo(){reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,null);}
    
    //6) Фаза разрешения троек
    public void initFarmerPhase(){
        currentPhaseSprite.sprite = phaseImageList[5];
        countOfThree = getDiceCountByValue(3);
        if(countOfThree > 0){   
            goToNextPhase();
            //reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
        else{
            if(countOfMushroomFarm < 1 && mushroomFarmsCellsList.Count < 1){ //вот эта хуйня спорная 
                //переходим в конец раунда
                goToNextPhase(phaseList[^1]);
            }
            else{
                //reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
                goToNextPhase();
            }
        }
    }
    public void clearResolveThree(){reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,null);}

    //7) Фаза разрешения четвёрок
    public void initDiggerPhase(){
        currentPhaseSprite.sprite = phaseImageList[6];
        currentPhaseSprite.color = Color.green;
        int countOfDiggers = getDiceCountByValue(4);
        int digCount = calculateDigCount(countOfDiggers);
        bool tmpMushroomFlag = false;
        if(countOfThree > 0){
            if(countOfDiggers > 0){
                DIG(digCount);
            }
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
        else if(countOfThree < 1){
            if(mushroomFarmsCellsList.Count > 0){
                DIG(digCount,true);
                tmpMushroomFlag = true;
            }
            else{
                reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
            } 
        }
        if( calculateBuildCount(getDiceCountByValue(5)) > 1 && getFreeCellsCountToBuild(tmpMushroomFlag)>0 && !calculatedPhaseList.Contains(phaseList[8])){
            Debug.Log("Added");
            calculatedPhaseList.Insert(calculatedPhaseList.Count-1,phaseList[8]);
        }
    }
    public int calculateDigCount(int diggers){
        if(countOfBlacksmith < 1){
            return diggers;
        }
        else{
            if(countOfBlacksmith < diggers){
                return (countOfBlacksmith*2)+(diggers-countOfBlacksmith);
            }
            else if(diggers <= countOfBlacksmith){
                return countOfBlacksmith*2;
            }
        }
        return 0;
    }

    public void DIG(int digCount,bool mushroomFarmFlag = false){ //main dig method
        int currentDigBalance = currentDepthReference.getDigBalance();
        
        if(currentDigBalance == digCount){
            fullDig(digCount,mushroomFarmFlag);
        }
        else if(currentDigBalance > digCount){ //не выходим за текущий слой
            digInsideDepth(digCount,mushroomFarmFlag);
        }
        else if(currentDigBalance < digCount){ //если вышли за границы текущего слоя
            digAndGoToNextDepth(digCount,mushroomFarmFlag); ////????       
        }
    }

    public void digInsideDepth(int digCount,bool mushroomFarmFlag = false){
        if(mushroomFarmFlag){
            if(!currentDepthReference.depthContainBuildings("MushroomFarm")){
                reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
                return;
            }
        }
        int tmpGemCount = 0;
        foreach(DepthRow row in currentDepthReference.rowList){
            if(row.currentCountOfDigToAccess != 0){
                int res = depthRowIntCompare(row,digCount);
                if(res < 0){ //прокопали и идём далее
                    digCount -= row.currentCountOfDigToAccess;
                    row.currentCountOfDigToAccess = 0;
                    row.accessToRowAllowed = true;
                    tmpGemCount += row.getCountOfDiggedGems();
                    messageToUserSystem.fillAndStartSequence(row.rowText,()=>{updateUiText(row.rowText,row.currentCountOfDigToAccess.ToString());});
                }
                else if(res == 0){
                    digCount = 0;
                    row.currentCountOfDigToAccess = 0;
                    row.accessToRowAllowed = true;
                    tmpGemCount += row.getCountOfDiggedGems();
                    messageToUserSystem.fillAndStartSequence(row.rowText,()=>{updateUiText(row.rowText,row.currentCountOfDigToAccess.ToString());currentPhaseSprite.color = Color.red;},true,messageToUserSystem.resolveAnimationSequenceList);
                    break;
                }
                else if (res > 0){
                    row.currentCountOfDigToAccess -= digCount;
                    tmpGemCount += row.getCountOfDiggedGems();
                    messageToUserSystem.fillAndStartSequence(row.rowText,()=>{updateUiText(row.rowText,row.currentCountOfDigToAccess.ToString());currentPhaseSprite.color = Color.red;},true,messageToUserSystem.resolveAnimationSequenceList);
                    break;
                }
            }
        }
        currentDepthReference.depthAvailable = true;
        reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
    }
    public void digAndGoToNextDepth(int digCount,bool mushroomFarmFlag = false){
        if(mushroomFarmFlag){
            fullDig(digCount,mushroomFarmFlag,false);   
        }
        else{
            int balanceOfDigCount = fullDig(digCount,mushroomFarmFlag,true);
            digInsideDepth(balanceOfDigCount,mushroomFarmFlag);
        }
    }
    public int fullDig(int digCount,bool mushroomFarmFlag = false,bool continueFillAnimationSequence = false){
        if(mushroomFarmFlag){
            if(!currentDepthReference.depthContainBuildings("MushroomFarm")){
                reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
                return 0;
            }
        }
        int tmpGetCount = 0;
        foreach(DepthRow row in currentDepthReference.rowList){
            if(row.currentCountOfDigToAccess != 0){ //чтобы снова не итерироваться по пустым элементам - добавляем это условие, копали ли мы слой ранее или нет
                digCount -= row.currentCountOfDigToAccess;
                row.currentCountOfDigToAccess = 0;
                tmpGetCount += row.dynamicGemLevelAccessList.Count;
                row.accessToRowAllowed = true;
                row.dynamicGemLevelAccessList.Clear();
                if(row != currentDepthReference.rowList[^1]){
                    messageToUserSystem.fillAndStartSequence(row.rowText,()=>{updateUiText(row.rowText,row.currentCountOfDigToAccess.ToString());});
                }
                else{
                    if(continueFillAnimationSequence){
                        messageToUserSystem.fillAndStartSequence(row.rowText,()=>{updateUiText(row.rowText,row.currentCountOfDigToAccess.ToString());});
                    }
                    else{
                        messageToUserSystem.fillAndStartSequence(row.rowText,()=>{updateUiText(row.rowText,row.currentCountOfDigToAccess.ToString());},true,messageToUserSystem.resolveAnimationSequenceList);
                    }
                }

            }
        }
        currentDepthReference.depthAvailable = true;
        setNextDepth();
        if(!continueFillAnimationSequence){
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
        return digCount;
    }
    public int depthRowIntCompare(DepthRow row,int digCount){
        int result = 0;
        if(row.currentCountOfDigToAccess > digCount){
            result = 1;
        }
        else if(row.currentCountOfDigToAccess < digCount){
            result = -1;
        }
        else if(row.currentCountOfDigToAccess == digCount){
            result = 0;
        }
        return result;
    }

    public void setNextDepth(){
        if(currentDepthReference != depthList[^1]){
            currentDepthReference = depthList[depthList.IndexOf(currentDepthReference)+1];
        }
    }

    public void clearResolveFourth(){}

    //8) Фаза разрешения пятёрок
    public void initBuilderPhase(){
        currentPhaseSprite.sprite = phaseImageList[7];
        int countOfFive = getDiceCountByValue(5);
        buildCount = calculateBuildCount(countOfFive);

        if(countOfThree > 0){ //нормальный переход в режим постройки
            checkForFivesValidation(buildCount);  
        }
        else {
            if(countOfMushroomFarm>0||mushroomFarmsCellsList.Count > 0){ //если троек нихуя нетно есть какие-никакие фермы
                checkForFivesValidation(buildCount,true);  
            }
        }
    }

    public void checkForFivesValidation(int _buildCount,bool mushroomflag=false){
        currentPhaseSprite.color = Color.white;
        if(_buildCount <= 1 || getFreeCellsCountToBuild(mushroomflag) == 0){
            foreach(Building b in shopBuildingsList){
                b.button.GetComponent<Image>().color = Color.white;
                b.MethodBeforeBuild = null;
                b.MiddleAction = null;
                b.MethodAfterBuild = null;
            }
            currentPhaseSprite.color = Color.red;
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
            return;
        }
        reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        subscribeShopButtonsForBuild(_buildCount,mushroomflag);
    }

    //каждуое строение которое можем позволить, подписываем на соответствующую функцию
    public void subscribeShopButtonsForBuild(int countOfBuilders,bool mushroomFlag){
        currentPhaseSprite.color = Color.green;
        foreach(Building b in shopBuildingsList){
            b.button.GetComponent<Image>().color = Color.white; //test
            b.MethodBeforeBuild = null;
            b.MiddleAction = null;
            b.MethodAfterBuild = null;
            if(b.props.cost <= countOfBuilders){
                b.button.GetComponent<Image>().color = Color.green;
                assignMethodsForShopButtonsBasedOnId(b,b.props.id);
            }
        }
        subUnsubFieldCellsToAction(mushroomFlag,true);
    }

    //в зависимости от id подписываем на соответствующие методы
    public void assignMethodsForShopButtonsBasedOnId(Building b,int id){
        switch(id){
            case 20: //шахта
                b.MiddleAction = ()=>{goToBuildMode(b);};
                b.MethodAfterBuild = () => {mineCellList.Add(b.parentCell);};
                break;
            case 21: //пивоварня
                b.MiddleAction = ()=>{goToBuildMode(b);};
                b.MethodAfterBuild = () => {increaseCounter(ref countOfBrew);};
                break;
            case 22: //таверна
                b.MethodBeforeBuild = ()=>{checkBuildingForUnique(b);};
                b.MiddleAction = ()=>{goToBuildMode(b);};
                b.MethodAfterBuild = () => {uniqueBuldingsList.Add(b);increaseCounter(ref countOfFreeReroll);};
                break;
            case 23: //статуя
                b.MethodBeforeBuild = ()=>{checkForGold(b,5);};
                b.MethodAfterBuild = ()=>{countOfStatues++;messageToUserSystem.updateCounterWithAnimation(goldText,()=>{goldCount -= 5;updateUiText(goldText,goldCount.ToString());});};
                break;
            case 30: //арена
                b.MiddleAction = ()=>{goToBuildMode(b);};
                b.MethodAfterBuild = () => {increaseCounter(ref countOfArenas);};
                break;
            case 31: //игорный дом
                b.MethodBeforeBuild = ()=>{checkBuildingForUnique(b);};
                b.MiddleAction = ()=>{goToBuildMode(b);};
                b.MethodAfterBuild = () => {uniqueBuldingsList.Add(b);increaseCounter(ref countOfFreeReroll);};
                break;
            case 32: //кузница
                b.MiddleAction = ()=>{goToBuildMode(b);};
                b.MethodAfterBuild = () => {increaseCounter(ref countOfBlacksmith);};
                break;
            case 33://мастерская столяра
                b.MiddleAction = ()=>{goToBuildMode(b);};
                b.MethodAfterBuild = () => {increaseCounter(ref countOfCarpenter);};
                break;
            case 40://грибная ферма
                b.MiddleAction = ()=>{goToBuildMode(b);};
                b.MethodAfterBuild = () => {mushroomFarmsCellsList.Add(b.parentCell);};
                break;
            case 41://убежище //ДОДЕЛАТЬ
                b.MethodBeforeBuild = ()=>{checkForGold(b,5);};
                b.MiddleAction = ()=> {goToBuildMode(b);};
                b.MethodAfterBuild = () => {
                                            messageToUserSystem.updateCounterWithAnimation(goldText,()=>{
                                                goldCount -= 5;
                                                updateUiText(goldText,goldCount.ToString());
                                            });
                                            int additionalPoints = b.parentCell.parentDepth.depthNumber * 5;
                                            points += additionalPoints;
                                            messageToUserSystem.updateCounterWithAnimation(pointsText,()=>{
                                                updateUiText(pointsText,points.ToString());
                                            });
                                            };
                break;
            case 42://концертный зал
                b.MethodBeforeBuild = ()=>{checkBuildingForUnique(b);};
                b.MiddleAction = ()=>{goToBuildMode(b);};
                b.MethodAfterBuild = () => {uniqueBuldingsList.Add(b);increaseCounter(ref countOfFreeReroll);};
                break;
            case 50: //тронный зал. Сам не до конца понял как эта хуета работает, подсмотреть потом на ютубе
                break;
        }
    }

    public void goToBuildMode(Building b){
        buildingReference = b;
        UIPanel.hideNamedPanel("shopPanel");
    }

    public void acceptCancelBuild(bool acceptFlag = true){
        if(!acceptFlag){ //если мы нажимаем на отмену стройки
            buildingReference = null;
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
            return;
        }
        else{ //если при нажатии мы хотим подтвердить стройку
            if(buildingReference.MethodAfterBuild != null){
                buildingReference.MethodAfterBuild();
            }
            subUnsubFieldCellsToAction(false,true);
            buildCount -= buildingReference.props.cost;
            buildingReference.parentCell.button.onClick.RemoveAllListeners();
            buildingReference.parentCell.buttonImg.color = Color.white;
            buildingReference.parentCell.parentDepth.buildingList.Add(buildingReference.parentCell);
            buildingReference = null;
            checkForFivesValidation(buildCount);
            reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        }
    }

    public void setUnsetBuilding(Cell c){
        if(buildingReference == null){
            return;
        }
        if(c.buildingInCell == null){
            if(buildingReference.setted && buildingReference.props.unique){ //если строение уже где-то установлено
                return;
            }
            else{ //а вот если строение нигде не установлено
                foreach(Cell tmpC in c.nearCells){
                    if(tmpC.buildingInCell!=null){
                        if(tmpC.buildingInCell.props.id == buildingReference.props.id){
                            return;
                        }
                    }
                }

                buildingReference.parentCell = c;

                c.buildingInCell = buildingReference;
                c.buttonImg.sprite = shopButtonAtlas.GetSprite(buildingReference.props.id.ToString());//buildingReference.props.icon;
                buildingReference.setted = true;
                UIPanel.showNamedPanel("mainPanel");
                reassignMainButtonNewAction(currentLanguage.getValueForButton("BUILD"),Color.green,()=>{acceptCancelBuild(true);});    
            }
        }
        else if(c.buildingInCell != null){
            buildingReference.parentCell = null;             
            c.buildingInCell = null;    
            c.buttonImg.sprite = null;
            buildingReference.setted = false;
            reassignMainButtonNewAction(currentLanguage.getValueForButton("CANCELBUILD"),Color.red,()=>{acceptCancelBuild(false);});
        }
    }

    public void checkBuildingForUnique(Building b){
        if(b.props.unique && uniqueBuldingsList.Contains(b)/*b.props.alreadyBuilded*/){
            b.MiddleAction = null;
            return;
        }
    }

    public void increaseCounter(ref int counter){
        counter += 1;
    }
    public void checkForGold(Building b,int requiredGold){
        if(goldCount < requiredGold){
            b.MiddleAction = null;
            return;
        }
        else{
            goToBuildMode(b);
        }
    }
    public int calculateBuildCount(int countOfFives){
        if(countOfCarpenter < 1){
            return countOfFives;
        }
        else if(countOfCarpenter >= 1){
            if(countOfCarpenter < countOfFives){
                return (countOfCarpenter * 2) + (countOfFives-countOfCarpenter);
            }
            else if(countOfCarpenter >= countOfFives){
                return countOfFives*2;
            }
        }
        return 0;
    }
    public void subUnsubFieldCellsToAction(bool mushroomFarmFlag = false,bool callback = false){
        if(!mushroomFarmFlag){ //работаем в обычном режиме
            foreach(Depth depth in depthList){
                if(depth.depthAvailable && depthList.IndexOf(depth) <= depthList.IndexOf(currentDepthReference)){
                    foreach(DepthRow row in depth.rowList){
                        if(row.accessToRowAllowed){
                            foreach(Cell c in row.cellList){
                                if(c.buildingInCell == null){
                                    if(callback == false){
                                        c.button.onClick.RemoveAllListeners();
                                        c.buttonImg.color = Color.white;
                                    }
                                    else{
                                        c.button.onClick.AddListener(()=>{setUnsetBuilding(c);});   
                                        c.buttonImg.color = Color.green;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else{ //если играем через флаг грибов
            foreach(Cell c in mushroomFarmsCellsList){
                foreach(DepthRow row in c.parentDepth.rowList){
                    if(row.accessToRowAllowed){
                        foreach(Cell cc in row.cellList){
                            if(cc.buildingInCell == null){
                                if(callback == false){
                                    cc.button.onClick.RemoveAllListeners();
                                    cc.buttonImg.color = Color.white;
                                }
                                else{
                                    cc.button.onClick.AddListener(()=>{setUnsetBuilding(cc);});   
                                    cc.buttonImg.color = Color.green;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    //Я хуй знает, мб херня снизу не будет работать как надо
    public int getFreeCellsCountToBuild(bool mushroomFarmFlag = false){ 
        int freeCells = 0;
        if(mushroomFarmFlag){
            foreach(Cell c in mushroomFarmsCellsList){
                foreach(DepthRow row in c.parentDepth.rowList){
                    if(row.accessToRowAllowed){
                        foreach(Cell cc in row.cellList){
                            if(cc.buildingInCell == null){
                                freeCells++;
                            }
                        }
                    }
                }
            }
            return freeCells;
        }
        else{
            foreach(Depth depth in depthList){
                if(depth.depthAvailable && depthList.IndexOf(depth) <= depthList.IndexOf(currentDepthReference)){
                    foreach(DepthRow row in depth.rowList){
                        if(row.accessToRowAllowed){
                            foreach(Cell c in row.cellList){
                                if(c.buildingInCell == null){
                                    freeCells++;
                                }
                            }
                        }
                        else{
                            break;
                        }
                    }
                }
                else{
                    break;
                }
            }
            return freeCells;
        } 
    }
    public void clearResolveFives(){

        //очищаем все клетки от методов
        foreach(Depth depth in depthList){
            if(depth.depthAvailable && depthList.IndexOf(depth) <= depthList.IndexOf(currentDepthReference)){
                foreach(DepthRow row in depth.rowList){
                    if(row.accessToRowAllowed){
                        foreach(Cell c in row.cellList){
                            c.button.onClick.RemoveAllListeners();
                            c.buttonImg.color = Color.white;
                        }
                    }
                    else{
                        break;
                    }
                }
            }
            else{
                break;
            }
        }
        
        //очищаем все кнопки магазина
        foreach(Building b in shopBuildingsList){
            b.button.GetComponent<Image>().color = Color.white;
            b.MethodBeforeBuild = null;
            b.MethodAfterBuild = null;
            b.MiddleAction = null;
            b.setted = false;
        }
        if(getDiceCountByValue(6) > 0 && mineCellList.Count > 0 && !calculatedPhaseList.Contains(phaseList[9])){
            calculatedPhaseList.Insert(calculatedPhaseList.Count-1,phaseList[9]);
        }
    }

    //9) Фаза разрешения шестёрок

    public void initMinersPhase(){
        currentPhaseSprite.sprite = phaseImageList[5];
        minersList = new List<Cell>();
        countOfSix = getDiceCountByValue(6);

        if(countOfThree > 0){ //ведём себя как обычно
            if(countOfSix < 1){
                reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
                return;
            }
            for(int i = 0;i<countOfSix;i++){
                minersList.Add(null);
            }

            foreach(Cell c in mineCellList){
                c.button.onClick.AddListener(()=>{setUnsetMiners(c);});
            }
        }
        else{
            if(mushroomFarmsCellsList.Count > 0){
                if(countOfSix < 1){
                    reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
                    return;
                }
                
                for(int i = 0;i<countOfSix;i++){
                    minersList.Add(null);
                }                
                foreach(Cell mushroomCell in mushroomFarmsCellsList){
                    foreach(DepthRow mushroomRow in mushroomCell.parentDepth.rowList){
                        if(mushroomRow.accessToRowAllowed){
                            foreach(Cell rowCell in mushroomRow.cellList){
                                if(rowCell.buildingInCell != null && rowCell.buildingInCell.props.id == 20){
                                    rowCell.button.onClick.AddListener(()=>{setUnsetMiners(rowCell);});
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void setUnsetMiners(Cell c){ //поработать ещё с этой хуйнёй
        
        if(minersList.Contains(c)){ //удаление
            
            c.buttonImg.color = Color.white;
            minersList[minersList.IndexOf(c)] = null;

            int countOfNullElements = 0;
            for(int i = 0;i< minersList.Count;i++){
                if(minersList[i]==null){
                    countOfNullElements++;
                }
            }

            if(countOfNullElements == countOfSix){
                reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
                return;
            }
            else{ //иначе подписываем кнопку на подтверждение добычи
                reassignMainButtonNewAction(currentLanguage.getValueForButton("MINE"),Color.yellow,acceptMining);   
                return;
            }
        }
        else if(!minersList.Contains(c)){ //добавление 
            int countOfNullElements = 0;
            for(int i = 0;i< minersList.Count;i++){
                if(minersList[i]==null){
                    countOfNullElements++;
                }
            }

            if(countOfNullElements <= countOfSix){ 
                for(int i = 0;i< minersList.Count;i++){ //проходимся по всему списку шахтёров и ищем первый попавшийся пустой элемент
                    if(minersList[i]==null){
                        minersList[i] = c;
                        minersList[i].buttonImg.color = Color.yellow;
                        reassignMainButtonNewAction(currentLanguage.getValueForButton("MINE"),Color.yellow,acceptMining);   
                        return;
                    }
                }                
            }
        }
    }

    public void acceptMining(){
        reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,()=>{goToNextPhase();});
        int tmpGold = 0;
        foreach(Cell c in minersList){
            if(c != null &&c.buildingInCell!=null && c.buildingInCell.props.id == 20){
                int random = UnityEngine.Random.Range(1,7);
                if(c.parentDepth.depthNumber < random){
                    tmpGold += c.parentDepth.goldIncome;
                }
                else{
                }

            }
        }
        minersList.Clear();
        messageToUserSystem.updateCounterWithAnimation(goldText,()=>{
            goldCount += tmpGold;
            updateUiText(goldText,goldCount.ToString());
        });
    }
    public void clearResolveSix(){
        foreach(Cell cellToClear in mineCellList){
            cellToClear.buttonImg.color = Color.white;
            cellToClear.button.onClick.RemoveAllListeners();
        }
        foreach(Cell c in minersList){
            c.button.onClick.RemoveAllListeners();
        }
        minersList.Clear();
        reassignMainButtonNewAction(currentLanguage.getValueForButton("NEXT"),Color.green,null);
    }

    //10 Фаза проверки конца игры
    /////////////////////////////////////ОСТАНОВИЛСЯ ТУТ
    public void checkEndGame(){
        roundCounter++;
        if(roundCounter > 20){
            points += countOfStatues == 0 ? 0 : countOfStatues * countOfStatues;
            endGamePointsText.text = $"Points\n{points.ToString()}";
            endGamePanel.DOFade(1,1f); 
            endGamePanel.blocksRaycasts = true;
            return;
        }

        messageToUserSystem.updateCounterWithAnimation(roundText,()=>{
            updateUiText(roundText,roundCounter.ToString());
            goToNextPhase();
            });
    }
    public void clearCheckEndGamePhase(){}
}